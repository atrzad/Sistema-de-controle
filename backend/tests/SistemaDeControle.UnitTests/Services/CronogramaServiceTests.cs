using Moq;
using SistemaDeControle.Application.Common.Exceptions;
using SistemaDeControle.Application.DTOs.Cronograma;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Application.Services;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;
using Xunit;

namespace SistemaDeControle.UnitTests.Services;

public class CronogramaServiceTests
{
    private readonly Mock<ICronogramaRepository> _cronogramaRepository = new();
    private readonly Mock<IProfessorRepository> _professorRepository = new();
    private readonly Mock<ITurmaRepository> _turmaRepository = new();
    private readonly Mock<ISalaRepository> _salaRepository = new();

    private CronogramaService CriarService() => new(
        _cronogramaRepository.Object, _professorRepository.Object, _turmaRepository.Object, _salaRepository.Object);

    private static Professor NovoProfessor(int id = 1) => new() { Id = id, Nome = "Prof. Teste", Email = "prof@teste.com", Matricula = "M1" };
    private static Turma NovaTurma(int id = 1) => new() { Id = id, Nome = "9A", AnoLetivo = 2026, Turno = Turno.Manha };
    private static Sala NovaSala(int id = 1) => new() { Id = id, Nome = "Sala 1" };

    private void ConfigurarReferenciasValidas(int professorId = 1, int turmaId = 1, int salaId = 1)
    {
        _professorRepository.Setup(r => r.GetByIdAsync(professorId, It.IsAny<CancellationToken>())).ReturnsAsync(NovoProfessor(professorId));
        _turmaRepository.Setup(r => r.GetByIdAsync(turmaId, It.IsAny<CancellationToken>())).ReturnsAsync(NovaTurma(turmaId));
        _salaRepository.Setup(r => r.GetByIdAsync(salaId, It.IsAny<CancellationToken>())).ReturnsAsync(NovaSala(salaId));
    }

    [Fact]
    public async Task CreateAsync_DeveLancarConflito_QuandoSalaJaTemAulaNoMesmoHorario()
    {
        ConfigurarReferenciasValidas();

        _cronogramaRepository
            .Setup(r => r.GetPorSalaEDiaAsync(1, DiaSemana.Segunda, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AulaAgendada>
            {
                new() { HoraInicio = new TimeOnly(8, 0), HoraFim = new TimeOnly(10, 0) }
            });

        _cronogramaRepository
            .Setup(r => r.GetPorProfessorEDiaAsync(1, DiaSemana.Segunda, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AulaAgendada>());

        var dto = new AulaAgendadaCreateDto(1, 1, 1, DiaSemana.Segunda, new TimeOnly(9, 0), new TimeOnly(11, 0), null, null, null);

        await Assert.ThrowsAsync<ConflictException>(() => CriarService().CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_DeveLancarConflito_QuandoProfessorJaTemAulaNoMesmoHorario()
    {
        ConfigurarReferenciasValidas();

        _cronogramaRepository
            .Setup(r => r.GetPorSalaEDiaAsync(1, DiaSemana.Segunda, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AulaAgendada>());

        _cronogramaRepository
            .Setup(r => r.GetPorProfessorEDiaAsync(1, DiaSemana.Segunda, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AulaAgendada>
            {
                new() { HoraInicio = new TimeOnly(8, 0), HoraFim = new TimeOnly(10, 0) }
            });

        var dto = new AulaAgendadaCreateDto(1, 1, 1, DiaSemana.Segunda, new TimeOnly(9, 0), new TimeOnly(11, 0), null, null, null);

        await Assert.ThrowsAsync<ConflictException>(() => CriarService().CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_NaoDeveLancarConflito_QuandoHorariosNaoSeSobrepoem()
    {
        ConfigurarReferenciasValidas();

        _cronogramaRepository
            .Setup(r => r.GetPorSalaEDiaAsync(1, DiaSemana.Segunda, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AulaAgendada>
            {
                new() { HoraInicio = new TimeOnly(8, 0), HoraFim = new TimeOnly(9, 0) }
            });

        _cronogramaRepository
            .Setup(r => r.GetPorProfessorEDiaAsync(1, DiaSemana.Segunda, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AulaAgendada>());

        var dto = new AulaAgendadaCreateDto(1, 1, 1, DiaSemana.Segunda, new TimeOnly(9, 0), new TimeOnly(11, 0), null, null, null);

        var resultado = await CriarService().CreateAsync(dto);

        Assert.Equal(1, resultado.ProfessorId);
        _cronogramaRepository.Verify(r => r.AddAsync(It.IsAny<AulaAgendada>(), It.IsAny<CancellationToken>()), Times.Once);
        _cronogramaRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DeveLancarNotFound_QuandoProfessorNaoExiste()
    {
        _professorRepository.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Professor?)null);

        var dto = new AulaAgendadaCreateDto(99, 1, 1, DiaSemana.Segunda, new TimeOnly(9, 0), new TimeOnly(11, 0), null, null, null);

        await Assert.ThrowsAsync<NotFoundException>(() => CriarService().CreateAsync(dto));
    }
}
