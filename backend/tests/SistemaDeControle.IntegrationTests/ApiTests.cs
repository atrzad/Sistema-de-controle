using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using SistemaDeControle.IntegrationTests.Infra;

namespace SistemaDeControle.IntegrationTests;

[Collection(ApiCollection.Nome)]
public class ApiTests
{
    private readonly ApiFactory _factory;

    public ApiTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Health_DeveVerificarBanco()
    {
        var resposta = await _factory.CreateClient().GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task Swagger_NaoDeveSerExpostoEmProducao()
    {
        // Sem Swagger a rota não existe; como toda rota exige login, a resposta é 401 (nunca a página).
        var resposta = await _factory.CreateClient().GetAsync("/swagger/index.html");
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);

        var client = await _factory.CriarClienteAutenticadoAsync();
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/swagger/index.html")).StatusCode);
    }

    [Fact]
    public async Task Endpoints_DevemExigirAutenticacao()
    {
        var resposta = await _factory.CreateClient().GetAsync("/api/v1/professores");
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Login_ComSenhaErrada_DeveRetornar401()
    {
        var resposta = await _factory.CreateClient().PostAsJsonAsync("/api/v1/auth/login",
            new { email = ApiFactory.AdminEmail, senha = "errada" }, ApiFactory.Json);
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Professor_ComEmailDuplicado_DeveRetornar409()
    {
        var client = await _factory.CriarClienteAutenticadoAsync();
        var id = Cenario.Unico("dup");
        var corpo = new { nome = "Prof", email = id + "@teste.local", matricula = id };

        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/v1/professores", corpo, ApiFactory.Json)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/v1/professores", corpo, ApiFactory.Json)).StatusCode);
    }

    [Fact]
    public async Task Cronograma_ComConflitoDeSala_DeveRetornar409_E_AulasEncostadasSaoPermitidas()
    {
        var client = await _factory.CriarClienteAutenticadoAsync();
        var sala = await Cenario.SalaAsync(client);
        var turma = await Cenario.TurmaAsync(client);
        var prof1 = await Cenario.ProfessorAsync(client);
        var prof2 = await Cenario.ProfessorAsync(client);

        await Cenario.CriarAsync(client, "/api/v1/cronograma", Cenario.Aula(prof1, turma, sala, "08:00:00", "09:00:00"));

        var sobreposta = await client.PostAsJsonAsync("/api/v1/cronograma", Cenario.Aula(prof2, turma, sala, "08:30:00", "09:30:00"), ApiFactory.Json);
        Assert.Equal(HttpStatusCode.Conflict, sobreposta.StatusCode);

        var encostada = await client.PostAsJsonAsync("/api/v1/cronograma", Cenario.Aula(prof2, turma, sala, "09:00:00", "10:00:00"), ApiFactory.Json);
        Assert.Equal(HttpStatusCode.Created, encostada.StatusCode);
    }

    [Fact]
    public async Task Cronograma_RequisicoesSimultaneasConflitantes_SoUmaDeveSerCriada()
    {
        var client = await _factory.CriarClienteAutenticadoAsync();
        var sala = await Cenario.SalaAsync(client);
        var turma = await Cenario.TurmaAsync(client);
        var professores = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Cenario.ProfessorAsync(client)));

        // Mesma sala e horário, professores diferentes, todas ao mesmo tempo: a checagem em
        // memória pode deixar passar mais de uma; a exclusion constraint do banco não.
        var respostas = await Task.WhenAll(professores.Select(p =>
            client.PostAsJsonAsync("/api/v1/cronograma", Cenario.Aula(p, turma, sala, "14:00:00", "15:00:00"), ApiFactory.Json)));

        Assert.Equal(1, respostas.Count(r => r.StatusCode == HttpStatusCode.Created));
        Assert.All(respostas.Where(r => r.StatusCode != HttpStatusCode.Created),
            r => Assert.Equal(HttpStatusCode.Conflict, r.StatusCode));
    }

    [Fact]
    public async Task Frequencia_Duplicada_DeveRetornar409()
    {
        var client = await _factory.CriarClienteAutenticadoAsync();
        var aula = await Cenario.CriarAsync(client, "/api/v1/cronograma", Cenario.Aula(
            await Cenario.ProfessorAsync(client), await Cenario.TurmaAsync(client), await Cenario.SalaAsync(client), "16:00:00", "17:00:00"));
        var corpo = new { aulaAgendadaId = aula, data = "2026-09-21", status = "Ausente" };

        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/v1/frequencia", corpo, ApiFactory.Json)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/v1/frequencia", corpo, ApiFactory.Json)).StatusCode);
    }

    [Fact]
    public async Task Justificativa_UploadDownloadERemocao()
    {
        var client = await _factory.CriarClienteAutenticadoAsync();
        var aula = await Cenario.CriarAsync(client, "/api/v1/cronograma", Cenario.Aula(
            await Cenario.ProfessorAsync(client), await Cenario.TurmaAsync(client), await Cenario.SalaAsync(client), "18:00:00", "19:00:00"));
        var registro = await Cenario.CriarAsync(client, "/api/v1/frequencia",
            new { aulaAgendadaId = aula, data = "2026-09-21", status = "AusenciaJustificada" });

        // Conteúdo que não é PDF/JPG/PNG é recusado mesmo com Content-Type de PDF.
        var falso = await client.PostAsync("/api/v1/justificativas", Upload(registro, "MZ-executavel"u8.ToArray()));
        Assert.Equal(HttpStatusCode.BadRequest, falso.StatusCode);

        var pdf = "%PDF-1.4\nconteudo de teste"u8.ToArray();
        var criada = await client.PostAsync("/api/v1/justificativas", Upload(registro, pdf));
        Assert.Equal(HttpStatusCode.Created, criada.StatusCode);
        var id = (await criada.Content.ReadFromJsonAsync<JsonElement>(ApiFactory.Json)).GetProperty("id").GetInt32();

        var download = await client.GetAsync($"/api/v1/justificativas/{id}/anexo");
        Assert.Equal(HttpStatusCode.OK, download.StatusCode);
        Assert.Equal("application/pdf", download.Content.Headers.ContentType?.MediaType);
        Assert.Equal(pdf, await download.Content.ReadAsByteArrayAsync());

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/v1/justificativas/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/v1/justificativas/{id}/anexo")).StatusCode);
    }

    [Fact]
    public async Task Frequencia_Paginada_DeveInformarTotal()
    {
        var client = await _factory.CriarClienteAutenticadoAsync();
        var resposta = await client.GetAsync("/api/v1/frequencia?page=1&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.True(resposta.Headers.Contains("X-Total-Count"));
        var itens = await resposta.Content.ReadFromJsonAsync<JsonElement>(ApiFactory.Json);
        Assert.True(itens.GetArrayLength() <= 1);
    }

    private static MultipartFormDataContent Upload(int registroId, byte[] conteudo)
    {
        var arquivo = new ByteArrayContent(conteudo);
        arquivo.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        return new MultipartFormDataContent
        {
            { new StringContent(registroId.ToString()), "RegistroFrequenciaId" },
            { new StringContent("Consulta médica"), "Motivo" },
            { arquivo, "Arquivo", "atestado.pdf" },
        };
    }
}
