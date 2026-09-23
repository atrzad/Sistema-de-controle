using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using SistemaDeControle.IntegrationTests.Infra;

namespace SistemaDeControle.IntegrationTests;

/// <summary>Testes que alteram a senha ou esgotam o limite de login usam uma API/banco próprios.</summary>
public class AuthTests : IAsyncLifetime
{
    private readonly ApiFactory _factory = new() { LimiteLogin = 5 };

    public Task InitializeAsync() => _factory.InitializeAsync();

    public Task DisposeAsync() => _factory.DisposeAsync();

    [Fact]
    public async Task AlterarSenha_DevePermitirLoginSomenteComANovaSenha()
    {
        var client = await _factory.CriarClienteAutenticadoAsync();

        var errada = await client.PostAsJsonAsync("/api/v1/auth/alterar-senha",
            new { senhaAtual = "incorreta", novaSenha = "NovaSenha2026x" }, ApiFactory.Json);
        Assert.Equal(HttpStatusCode.BadRequest, errada.StatusCode);

        var ok = await client.PostAsJsonAsync("/api/v1/auth/alterar-senha",
            new { senhaAtual = ApiFactory.AdminSenha, novaSenha = "NovaSenha2026x" }, ApiFactory.Json);
        Assert.Equal(HttpStatusCode.NoContent, ok.StatusCode);

        var anonimo = _factory.CreateClient();
        var antiga = await anonimo.PostAsJsonAsync("/api/v1/auth/login",
            new { email = ApiFactory.AdminEmail, senha = ApiFactory.AdminSenha }, ApiFactory.Json);
        Assert.Equal(HttpStatusCode.Unauthorized, antiga.StatusCode);
    }

    [Fact]
    public async Task ContaAdmin_ComRedefinicaoLigada_VoltaParaASenhaConfigurada()
    {
        var client = await _factory.CriarClienteAutenticadoAsync();
        var trocada = await client.PostAsJsonAsync("/api/v1/auth/alterar-senha",
            new { senhaAtual = ApiFactory.AdminSenha, novaSenha = "SenhaEsquecida99" }, ApiFactory.Json);
        Assert.Equal(HttpStatusCode.NoContent, trocada.StatusCode);

        // Reinicia a API com a redefinição ligada, contra o mesmo banco.
        await using var reiniciada = _factory.WithWebHostBuilder(b => b.UseSetting("Seed:ResetAdminPassword", "true"));
        var resposta = await reiniciada.CreateClient().PostAsJsonAsync("/api/v1/auth/login",
            new { email = ApiFactory.AdminEmail, senha = ApiFactory.AdminSenha }, ApiFactory.Json);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task Login_DeveSerLimitadoPorTaxa()
    {
        var client = _factory.CreateClient();
        var status = new List<HttpStatusCode>();

        for (var i = 0; i < 8; i++)
        {
            var resposta = await client.PostAsJsonAsync("/api/v1/auth/login",
                new { email = ApiFactory.AdminEmail, senha = "errada" }, ApiFactory.Json);
            status.Add(resposta.StatusCode);
        }

        Assert.Contains(HttpStatusCode.TooManyRequests, status);
    }
}
