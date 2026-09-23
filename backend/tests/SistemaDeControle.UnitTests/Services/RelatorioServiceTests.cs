using Moq;
using SistemaDeControle.Application.Common;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Application.Services;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;
using Xunit;

namespace SistemaDeControle.UnitTests.Services;

public class RelatorioServiceTests
{
    private readonly Mock<IFrequenciaRepository> _frequenciaRepository = new();
    private readonly Mock<IProfessorRepository> _professorRepository = new();

    private RelatorioService CriarService() => new(_frequenciaRepository.Object, _professorRepository.Object);

    [Fact]
    public async Task GetRelatorioConsolidadoAsync_DeveCalcularTotaisPorProfessorComUmaUnicaConsulta()
    {
        var inicio = new DateOnly(2026, 9, 1);
        var fim = new DateOnly(2026, 9, 30);

        _professorRepository
            .Setup(r => r.ListAsync(null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Professor>
            {
                new() { Id = 1, Nome = "Ana" },
                new() { Id = 2, Nome = "Bruno" },
            });

        _frequenciaRepository
            .Setup(r => r.ContarPorProfessorEStatusAsync(null, inicio, fim, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ContagemFrequencia>
            {
                new(1, StatusFrequencia.Presente, 3),
                new(1, StatusFrequencia.Ausente, 1),
            });

        var resultado = await CriarService().GetRelatorioConsolidadoAsync(inicio, fim);

        var ana = resultado.Single(r => r.ProfessorId == 1);
        Assert.Equal(4, ana.TotalAulas);
        Assert.Equal(3, ana.TotalPresencas);
        Assert.Equal(1, ana.TotalAusencias);
        Assert.Equal(75, ana.PercentualPresenca);

        var bruno = resultado.Single(r => r.ProfessorId == 2);
        Assert.Equal(0, bruno.TotalAulas);
        Assert.Equal(0, bruno.PercentualPresenca);

        _frequenciaRepository.Verify(
            r => r.ContarPorProfessorEStatusAsync(It.IsAny<int?>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetRelatorioSemanalAsync_DeveUsarSegundaADomingoDaSemanaDeReferencia()
    {
        _professorRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Professor { Id = 1, Nome = "Ana" });
        _frequenciaRepository
            .Setup(r => r.ContarPorProfessorEStatusAsync(1, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ContagemFrequencia>());

        // 2026-09-23 é uma quarta-feira.
        var resultado = await CriarService().GetRelatorioSemanalAsync(1, new DateOnly(2026, 9, 23));

        Assert.Equal(new DateOnly(2026, 9, 21), resultado.DataInicio);
        Assert.Equal(new DateOnly(2026, 9, 27), resultado.DataFim);
    }
}
