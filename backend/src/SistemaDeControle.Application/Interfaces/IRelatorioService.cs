using SistemaDeControle.Application.DTOs.Frequencia;
using SistemaDeControle.Application.DTOs.Relatorios;

namespace SistemaDeControle.Application.Interfaces;

public interface IRelatorioService
{
    Task<RelatorioFrequenciaDto> GetRelatorioSemanalAsync(int professorId, DateOnly dataReferencia, CancellationToken cancellationToken = default);
    Task<RelatorioFrequenciaDto> GetRelatorioMensalAsync(int professorId, int mes, int ano, CancellationToken cancellationToken = default);
    Task<List<RelatorioFrequenciaDto>> GetRelatorioConsolidadoAsync(DateOnly dataInicio, DateOnly dataFim, CancellationToken cancellationToken = default);
    Task<List<TurmaAfetadaDto>> GetTurmasAfetadasHistoricoAsync(DateOnly dataInicio, DateOnly dataFim, CancellationToken cancellationToken = default);
}
