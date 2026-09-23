using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Npgsql;

namespace SistemaDeControle.Api.Desktop;

/// <summary>
/// Modo desktop (pacote .exe): sobe um PostgreSQL portátil que acompanha o executável,
/// serve o frontend pela própria API e abre o navegador. Só é ativado no build com
/// -p:Desktop=true (constante DESKTOP); a versão de servidor/Docker não usa esta classe.
///
/// Layout esperado ao lado do executável:
///   pgsql/    binários do PostgreSQL (bin/, lib/, share/)
///   wwwroot/  build do frontend
/// Dados do usuário (banco, anexos, configuração) ficam em %LOCALAPPDATA%\SistemaDeControle.
/// </summary>
public sealed class DesktopHost
{
    private const string BancoNome = "sistema_controle";
    private const string ArgRedefinirSenha = "--redefinir-senha";

    private readonly ConfiguracaoDesktop _config;
    private readonly string? _senhaAdminInformada;
    private readonly bool _redefinirSenha;

    public static string PastaDados { get; } = Environment.GetEnvironmentVariable("SDC_PASTA_DADOS")
        ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemaDeControle");

    private static string PastaPostgres => Path.Combine(AppContext.BaseDirectory, "pgsql");
    private static string PastaBanco => Path.Combine(PastaDados, "banco");
    private static string ArquivoConfig => Path.Combine(PastaDados, "config.json");

    public string UrlAplicacao => $"http://localhost:{_config.PortaAplicacao}";

    private DesktopHost(ConfiguracaoDesktop config, string? senhaAdminInformada, bool redefinirSenha)
    {
        _config = config;
        _senhaAdminInformada = senhaAdminInformada;
        _redefinirSenha = redefinirSenha;
    }

    /// <summary>Argumentos próprios do modo desktop, removidos antes de chegar ao ASP.NET.</summary>
    public static string[] FiltrarArgumentos(string[] args) =>
        args.Where(a => !a.Equals(ArgRedefinirSenha, StringComparison.OrdinalIgnoreCase)).ToArray();

    /// <returns>null quando o sistema não deve subir (já aberto em outra janela ou porta ocupada).</returns>
    public static async Task<DesktopHost?> IniciarAsync(string[] args)
    {
        try
        {
            Console.OutputEncoding = Encoding.UTF8;
        }
        catch (IOException)
        {
            // Sem console real (saída redirecionada): mantém a codificação padrão.
        }

        // Aberto com dois cliques, a janela fecharia antes de dar para ler o erro.
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            Console.WriteLine();
            Console.WriteLine("ERRO: o sistema não pôde continuar.");
            Console.WriteLine((e.ExceptionObject as Exception)?.Message);
            Console.WriteLine($"Detalhes do banco em: {Path.Combine(PastaDados, "postgres.log")}");
            AguardarTecla();
        };

        Console.WriteLine("=== Sistema de Controle ===");
        Console.WriteLine($"Dados em: {PastaDados}");
        Console.WriteLine();

        Directory.CreateDirectory(PastaDados);

        var redefinir = args.Any(a => a.Equals(ArgRedefinirSenha, StringComparison.OrdinalIgnoreCase));
        string? senha = null;

        var config = LerConfig();
        var porta = config?.PortaAplicacao ?? new ConfiguracaoDesktop().PortaAplicacao;
        if (!await VerificarPortaLivreAsync(porta))
            return null;

        if (config is null)
        {
            Console.WriteLine("Primeira execução: vamos criar a conta de administrador.");
            var email = Perguntar("E-mail do administrador", "pedagogo@sistemadecontrole.local");
            senha = PerguntarNovaSenha();

            config = new ConfiguracaoDesktop
            {
                AdminEmail = email,
                SenhaPostgres = GerarSegredo(24),
                SegredoJwt = GerarSegredo(48),
            };
            SalvarConfig(config);
        }
        else if (redefinir)
        {
            Console.WriteLine($"Redefinindo a senha da conta {config.AdminEmail}.");
            senha = PerguntarNovaSenha();
        }

        var host = new DesktopHost(config, senha, redefinir);
        await host.IniciarPostgresAsync();
        return host;
    }

    public void ConfigurarBuilder(WebApplicationBuilder builder)
    {
        builder.WebHost.UseUrls(UrlAplicacao);

        var valores = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = StringConexao(BancoNome),
            ["Jwt:Secret"] = _config.SegredoJwt,
            ["Storage:BasePath"] = Path.Combine(PastaDados, "anexos"),
            ["Seed:AdminEmail"] = _config.AdminEmail,
            ["Seed:AdminPassword"] = _senhaAdminInformada,
            ["Seed:ResetAdminPassword"] = _redefinirSenha ? "true" : "false",
            ["Cors:AllowedOrigin"] = UrlAplicacao,
        };
        builder.Configuration.AddInMemoryCollection(valores);
    }

    /// <summary>Middlewares de arquivos estáticos — antes de autenticação, para o frontend carregar sem login.</summary>
    public void ConfigurarArquivosEstaticos(WebApplication app)
    {
        app.UseDefaultFiles();
        app.UseStaticFiles();
    }

    public void ConfigurarEndpoints(WebApplication app)
    {
        // Rotas do React (ex.: /professores ao recarregar a página) caem no index.html.
        app.MapFallbackToFile("index.html").AllowAnonymous();

        var lifetime = app.Lifetime;
        lifetime.ApplicationStarted.Register(() =>
        {
            Console.WriteLine();
            Console.WriteLine($"Sistema no ar em {UrlAplicacao}");
            Console.WriteLine("Mantenha esta janela aberta enquanto usa o sistema. Para encerrar, feche-a ou pressione Ctrl+C.");
            AbrirNavegador(UrlAplicacao);
        });
        lifetime.ApplicationStopping.Register(PararPostgres);
    }

    /// <summary>
    /// Dois cliques no .exe com o sistema já aberto apenas abrem o navegador de novo,
    /// em vez de falhar ao tentar usar a mesma porta.
    /// </summary>
    private static async Task<bool> VerificarPortaLivreAsync(int porta)
    {
        try
        {
            var ouvinte = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, porta);
            ouvinte.Start();
            ouvinte.Stop();
            return true;
        }
        catch (System.Net.Sockets.SocketException)
        {
        }

        var url = $"http://localhost:{porta}";
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        try
        {
            if ((await http.GetAsync($"{url}/health")).IsSuccessStatusCode)
            {
                Console.WriteLine($"O sistema já está aberto em outra janela. Abrindo {url} no navegador.");
                AbrirNavegador(url);
                return false;
            }
        }
        catch (Exception)
        {
        }

        Console.WriteLine($"A porta {porta} está sendo usada por outro programa.");
        Console.WriteLine($"Feche esse programa ou altere \"PortaAplicacao\" em {ArquivoConfig}.");
        AguardarTecla();
        return false;
    }

    private static void AguardarTecla()
    {
        if (Console.IsInputRedirected) return;
        Console.WriteLine("Pressione qualquer tecla para sair.");
        Console.ReadKey(intercept: true);
    }

    private string StringConexao(string banco) =>
        $"Host=localhost;Port={_config.PortaPostgres};Database={banco};Username=postgres;Password={_config.SenhaPostgres}";

    private async Task IniciarPostgresAsync()
    {
        if (!Directory.Exists(PastaPostgres))
            throw new InvalidOperationException($"Pasta do PostgreSQL não encontrada: {PastaPostgres}");

        if (!File.Exists(Path.Combine(PastaBanco, "PG_VERSION")))
        {
            Console.WriteLine("Criando banco de dados local (só na primeira vez)...");
            var arquivoSenha = Path.Combine(PastaDados, "pwfile.tmp");
            await File.WriteAllTextAsync(arquivoSenha, _config.SenhaPostgres);
            try
            {
                await ExecutarAsync("initdb",
                    "-D", PastaBanco, "-U", "postgres", "--pwfile", arquivoSenha,
                    // ICU pt-BR: buscas sem diferenciar maiúsculas funcionam com acentos
                    // ("CONCEIÇÃO" encontra "Conceição"), independente do idioma do Windows.
                    "-E", "UTF8", "--locale-provider=icu", "--icu-locale=pt-BR", "--locale=C",
                    "-A", "scram-sha-256");
            }
            finally
            {
                File.Delete(arquivoSenha);
            }
        }

        var status = await ExecutarAsync("pg_ctl", permitirFalha: true, "status", "-D", PastaBanco);
        if (status != 0)
        {
            Console.WriteLine("Iniciando banco de dados...");
            await ExecutarAsync("pg_ctl", permitirFalha: false, redirecionarSaida: false,
                "start", "-w", "-t", "60", "-D", PastaBanco,
                "-l", Path.Combine(PastaDados, "postgres.log"),
                "-o", $"-p {_config.PortaPostgres} -c listen_addresses=localhost");
        }

        await using var conexao = new NpgsqlConnection(StringConexao("postgres"));
        await conexao.OpenAsync();
        await using var existe = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @nome", conexao);
        existe.Parameters.AddWithValue("nome", BancoNome);
        if (await existe.ExecuteScalarAsync() is null)
        {
            await using var criar = new NpgsqlCommand($"CREATE DATABASE {BancoNome}", conexao);
            await criar.ExecuteNonQueryAsync();
        }
    }

    private void PararPostgres()
    {
        Console.WriteLine("Encerrando banco de dados...");
        try
        {
            ExecutarAsync("pg_ctl", permitirFalha: true, "stop", "-D", PastaBanco, "-m", "fast", "-w").GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Não foi possível parar o PostgreSQL: {ex.Message}");
        }
    }

    private static Task<int> ExecutarAsync(string programa, params string[] argumentos) =>
        ExecutarAsync(programa, permitirFalha: false, argumentos);

    private static Task<int> ExecutarAsync(string programa, bool permitirFalha, params string[] argumentos) =>
        ExecutarAsync(programa, permitirFalha, redirecionarSaida: true, argumentos);

    /// <param name="redirecionarSaida">
    /// Deve ser false para "pg_ctl start": o postgres herda os handles de saída e mantê-los
    /// redirecionados faria a leitura esperar até o banco ser desligado.
    /// </param>
    private static async Task<int> ExecutarAsync(string programa, bool permitirFalha, bool redirecionarSaida, params string[] argumentos)
    {
        var executavel = Path.Combine(PastaPostgres, "bin", OperatingSystem.IsWindows() ? programa + ".exe" : programa);
        var inicio = new ProcessStartInfo(executavel)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = redirecionarSaida,
            RedirectStandardError = redirecionarSaida,
        };
        foreach (var argumento in argumentos)
            inicio.ArgumentList.Add(argumento);

        using var processo = Process.Start(inicio)
            ?? throw new InvalidOperationException($"Não foi possível executar {executavel}");

        var saida = redirecionarSaida ? processo.StandardOutput.ReadToEndAsync() : Task.FromResult("");
        var erro = redirecionarSaida ? processo.StandardError.ReadToEndAsync() : Task.FromResult("");
        await processo.WaitForExitAsync();

        if (processo.ExitCode != 0 && !permitirFalha)
            throw new InvalidOperationException(
                $"{programa} terminou com código {processo.ExitCode}.\n{await saida}\n{await erro}\nVeja também {Path.Combine(PastaDados, "postgres.log")}");

        return processo.ExitCode;
    }

    private static void AbrirNavegador(string url)
    {
        if (Environment.GetEnvironmentVariable("SDC_SEM_NAVEGADOR") == "1")
            return;

        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch
        {
            Console.WriteLine($"Abra no navegador: {url}");
        }
    }

    private static string Perguntar(string pergunta, string padrao)
    {
        Console.Write($"{pergunta} [{padrao}]: ");
        var resposta = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(resposta) ? padrao : resposta;
    }

    private static string PerguntarNovaSenha()
    {
        while (true)
        {
            var senha = LerSenha("Nova senha (mín. 10 caracteres, com letras e números): ");
            if (senha.Length < 10 || !senha.Any(char.IsLetter) || !senha.Any(char.IsDigit))
            {
                Console.WriteLine("Senha fraca. Tente novamente.");
                continue;
            }

            if (LerSenha("Confirme a senha: ") != senha)
            {
                Console.WriteLine("As senhas não conferem. Tente novamente.");
                continue;
            }

            return senha;
        }
    }

    private static string LerSenha(string rotulo)
    {
        Console.Write(rotulo);

        // Entrada redirecionada (scripts/testes): lê a linha inteira.
        if (Console.IsInputRedirected)
        {
            var linha = Console.ReadLine() ?? "";
            Console.WriteLine();
            return linha;
        }

        var senha = new StringBuilder();
        while (true)
        {
            var tecla = Console.ReadKey(intercept: true);
            if (tecla.Key == ConsoleKey.Enter) break;
            if (tecla.Key == ConsoleKey.Backspace)
            {
                if (senha.Length > 0)
                {
                    senha.Length--;
                    Console.Write("\b \b");
                }
                continue;
            }
            if (!char.IsControl(tecla.KeyChar))
            {
                senha.Append(tecla.KeyChar);
                Console.Write('*');
            }
        }
        Console.WriteLine();
        return senha.ToString();
    }

    private static string GerarSegredo(int bytes) =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(bytes)).Replace('+', '-').Replace('/', '_').TrimEnd('=');

    private static ConfiguracaoDesktop? LerConfig() =>
        File.Exists(ArquivoConfig)
            ? JsonSerializer.Deserialize<ConfiguracaoDesktop>(File.ReadAllText(ArquivoConfig))
            : null;

    private static void SalvarConfig(ConfiguracaoDesktop config) =>
        File.WriteAllText(ArquivoConfig, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

    private sealed class ConfiguracaoDesktop
    {
        public string AdminEmail { get; set; } = "";
        public string SenhaPostgres { get; set; } = "";
        public string SegredoJwt { get; set; } = "";
        public int PortaPostgres { get; set; } = 54329;
        public int PortaAplicacao { get; set; } = 5080;
    }
}
