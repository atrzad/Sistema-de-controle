using Moq;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Application.Services;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;
using Xunit;

namespace SistemaDeControle.UnitTests.Services;

public class FrequenciaServiceTests
{
    private readonly Mock<IFrequenciaRepository> _frequenciaRepository = new();
    private readonly Mock<ICronogramaRepository> _cronogramaRepository = new();

    private FrequenciaService CriarService() => new(_frequenciaRepository.Object, _cronogramaRepository.Object);

    private static RegistroFrequencia NovoRegistro(int id, StatusFrequencia status, int professorId = 1)
    {
        var professor = new Professor { Id = professorId, Nome = "Prof. Teste", Email = "p@t.com", Matricula = "M1" };
        var turma = new Turma { Id = 1, Nome = "9A", AnoLetivo = 2026, Turno = Turno.Manha };
        var sala = new Sala { Id = 1, Nome = "Sala 1" };
        var aula = new AulaAgendada
        {
            Id = 1, Professor = professor, ProfessorId = professorId, Turma = turma, TurmaId = 1, Sala = sala, SalaId = 1,
            DiaSemana = DiaSemana.Segunda, HoraInicio = new TimeOnly(8, 0), HoraFim = new TimeOnly(9, 0),
        };

        return new RegistroFrequencia
        {
            Id = id, AulaAgendadaId = 1, AulaAgendada = aula, ProfessorId = professorId, Professor = professor,
            Data = new DateOnly(2026, 9, 21), Status = status,
        };
    }

    [Fact]
    public async Task GetTurmasAfetadasAsync_DeveRetornarApenasAusenciasENaoPresencas()
    {
        var data = new DateOnly(2026, 9, 21);
        _frequenciaRepository
            .Setup(r => r.ListByDataAsync(data, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RegistroFrequencia>
            {
                NovoRegistro(1, StatusFrequencia.Presente),
                NovoRegistro(2, StatusFrequencia.Ausente),
                NovoRegistro(3, StatusFrequencia.AusenciaJustificada),
            });

        var resultado = await CriarService().GetTurmasAfetadasAsync(data, null);

        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, r => Assert.NotEqual(StatusFrequencia.Presente, r.Status));
    }

    [Fact]
    public async Task GetTurmasAfetadasAsync_DeveFiltrarPorProfessor()
    {
        var data = new DateOnly(2026, 9, 21);
        _frequenciaRepository
            .Setup(r => r.ListByDataAsync(data, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RegistroFrequencia>
            {
                NovoRegistro(1, StatusFrequencia.Ausente, professorId: 1),
                NovoRegistro(2, StatusFrequencia.Ausente, professorId: 2),
            });

        var resultado = await CriarService().GetTurmasAfetadasAsync(data, professorId: 2);

        Assert.Single(resultado);
        Assert.Equal(2, resultado[0].ProfessorId);
    }
}
