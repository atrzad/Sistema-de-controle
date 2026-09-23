using System.Net.Http.Json;
using System.Text.Json;

namespace SistemaDeControle.IntegrationTests.Infra;

/// <summary>Cria dados de apoio com nomes únicos, para os testes não colidirem entre si.</summary>
public static class Cenario
{
    public static string Unico(string prefixo) => $"{prefixo}-{Guid.NewGuid():N}";

    public static async Task<int> CriarAsync(HttpClient client, string url, object corpo)
    {
        var resposta = await client.PostAsJsonAsync(url, corpo, ApiFactory.Json);
        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException($"POST {url} falhou: {(int)resposta.StatusCode} {await resposta.Content.ReadAsStringAsync()}");

        var json = await resposta.Content.ReadFromJsonAsync<JsonElement>(ApiFactory.Json);
        return json.GetProperty("id").GetInt32();
    }

    public static Task<int> ProfessorAsync(HttpClient client)
    {
        var id = Unico("p");
        return CriarAsync(client, "/api/v1/professores", new { nome = "Prof " + id, email = id + "@teste.local", matricula = id });
    }

    public static Task<int> TurmaAsync(HttpClient client) =>
        CriarAsync(client, "/api/v1/turmas", new { nome = Unico("t"), turno = "Manha", anoLetivo = 2026 });

    public static Task<int> SalaAsync(HttpClient client) =>
        CriarAsync(client, "/api/v1/salas", new { nome = Unico("s"), capacidade = 30 });

    public static object Aula(int professorId, int turmaId, int salaId, string inicio = "08:00:00", string fim = "09:00:00") => new
    {
        professorId, turmaId, salaId, diaSemana = "Segunda", horaInicio = inicio, horaFim = fim,
    };
}
