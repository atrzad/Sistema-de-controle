using SistemaDeControle.Application.Common;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Application.Interfaces;

public interface IJustificativaRepository
{
    Task<Justificativa?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Justificativa?> GetByRegistroFrequenciaIdAsync(int registroFrequenciaId, CancellationToken cancellationToken = default);

    Task<ResultadoPaginado<Justificativa>> ListAsync(
        int? professorId, DateOnly? dataInicio, DateOnly? dataFim, Paginacao? paginacao,
        CancellationToken cancellationToken = default);

    Task AddAsync(Justificativa justificativa, CancellationToken cancellationToken = default);
    void Remove(Justificativa justificativa);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
