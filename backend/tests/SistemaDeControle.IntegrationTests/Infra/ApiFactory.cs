using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;

namespace SistemaDeControle.IntegrationTests.Infra;

/// <summary>
/// Sobe a API real (ambiente Production) contra um Postgres descartável.
/// Por padrão usa um container (Testcontainers, requer Docker). Se SDC_TEST_PG estiver
/// definida com a connection string de um Postgres já rodando, cria um banco temporário
/// nele — útil sem Docker, ex.: com o Postgres portátil de scripts/build-desktop.sh.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string AdminEmail = "admin@teste.local";
    public const string AdminSenha = "SenhaInicial123";

    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private static readonly string? PostgresExterno = Environment.GetEnvironmentVariable("SDC_TEST_PG");

    private readonly PostgreSqlContainer? _postgres = PostgresExterno is null ? new PostgreSqlBuilder("postgres:16-alpine").Build() : null;
    private readonly string _bancoTemporario = "sdc_teste_" + Guid.NewGuid().ToString("N");
    private readonly string _storage = Path.Combine(Path.GetTempPath(), "sdc-tests-" + Guid.NewGuid().ToString("N"));

    public int LimiteLogin { get; init; } = 1000;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");
        builder.UseSetting("ConnectionStrings:DefaultConnection", ConnectionString);
        builder.UseSetting("Jwt:Secret", "chave-de-teste-com-mais-de-32-caracteres-123456");
        builder.UseSetting("Seed:AdminEmail", AdminEmail);
        builder.UseSetting("Seed:AdminPassword", AdminSenha);
        builder.UseSetting("Storage:BasePath", _storage);
        builder.UseSetting("RateLimiting:LoginPermitLimit", LimiteLogin.ToString());
    }

    public async Task<HttpClient> CriarClienteAutenticadoAsync(string email = AdminEmail, string senha = AdminSenha)
    {
        var client = CreateClient();
        var resposta = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, senha }, Json);
        resposta.EnsureSuccessStatusCode();
        var login = await resposta.Content.ReadFromJsonAsync<JsonElement>(Json);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.GetProperty("token").GetString());
        return client;
    }

    private string ConnectionString => _postgres is not null
        ? _postgres.GetConnectionString()
        : new NpgsqlConnectionStringBuilder(PostgresExterno) { Database = _bancoTemporario }.ConnectionString;

    public async Task InitializeAsync()
    {
        if (_postgres is not null)
        {
            await _postgres.StartAsync();
            return;
        }

        await ExecutarNoPostgresExternoAsync($"CREATE DATABASE {_bancoTemporario}");
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();

        if (_postgres is not null)
            await _postgres.DisposeAsync();
        else
            await ExecutarNoPostgresExternoAsync($"DROP DATABASE IF EXISTS {_bancoTemporario} WITH (FORCE)");

        if (Directory.Exists(_storage)) Directory.Delete(_storage, recursive: true);
    }

    private static async Task ExecutarNoPostgresExternoAsync(string sql)
    {
        await using var conexao = new NpgsqlConnection(PostgresExterno);
        await conexao.OpenAsync();
        await using var comando = new NpgsqlCommand(sql, conexao);
        await comando.ExecuteNonQueryAsync();
    }
}

[CollectionDefinition(Nome)]
public class ApiCollection : ICollectionFixture<ApiFactory>
{
    public const string Nome = "api";
}
