using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.Interfaces;

public interface ITurmaRepository
{
    Task<Turma?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Turma>> ListAsync(int? anoLetivo, Turno? turno, bool? ativo, CancellationToken cancellationToken = default);
    Task AddAsync(Turma turma, CancellationToken cancellationToken = default);
    void Remove(Turma turma);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
