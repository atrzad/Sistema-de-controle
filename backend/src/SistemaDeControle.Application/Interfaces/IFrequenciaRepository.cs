using SistemaDeControle.Application.Common;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.Interfaces;

public interface IFrequenciaRepository
{
    Task<RegistroFrequencia?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RegistroFrequencia?> GetByAulaEDataAsync(int aulaAgendadaId, DateOnly data, CancellationToken cancellationToken = default);

    Task<List<RegistroFrequencia>> ListAsync(
        int? professorId, DateOnly? dataInicio, DateOnly? dataFim, StatusFrequencia? status,
        CancellationToken cancellationToken = default);

    Task<ResultadoPaginado<RegistroFrequencia>> ListPaginadoAsync(
        int? professorId, DateOnly? dataInicio, DateOnly? dataFim, StatusFrequencia? status, Paginacao? paginacao,
        CancellationToken cancellationToken = default);

    /// <summary>Contagem de registros por professor e status no período, agregada no banco.</summary>
    Task<List<ContagemFrequencia>> ContarPorProfessorEStatusAsync(
        int? professorId, DateOnly dataInicio, DateOnly dataFim,
        CancellationToken cancellationToken = default);

    Task<List<RegistroFrequencia>> ListByDataAsync(DateOnly data, CancellationToken cancellationToken = default);

    Task AddAsync(RegistroFrequencia registro, CancellationToken cancellationToken = default);
    void Remove(RegistroFrequencia registro);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
