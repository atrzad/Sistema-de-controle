using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace SistemaDeControle.IntegrationTests.Infra;

/// <summary>
/// Sobe a API real (ambiente Production) contra um Postgres descartável em container.
/// Requer Docker rodando na máquina/CI.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string AdminEmail = "admin@teste.local";
    public const string AdminSenha = "SenhaInicial123";

    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
    private readonly string _storage = Path.Combine(Path.GetTempPath(), "sdc-tests-" + Guid.NewGuid().ToString("N"));

    public int LimiteLogin { get; init; } = 1000;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");
        builder.UseSetting("ConnectionStrings:DefaultConnection", _postgres.GetConnectionString());
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

    public Task InitializeAsync() => _postgres.StartAsync();

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
        if (Directory.Exists(_storage)) Directory.Delete(_storage, recursive: true);
    }
}

[CollectionDefinition(Nome)]
public class ApiCollection : ICollectionFixture<ApiFactory>
{
    public const string Nome = "api";
}
