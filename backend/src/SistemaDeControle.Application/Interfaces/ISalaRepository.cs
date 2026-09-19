using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Application.Interfaces;

public interface ISalaRepository
{
    Task<Sala?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Sala>> ListAsync(bool? ativo, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNomeAsync(string nome, int? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Sala sala, CancellationToken cancellationToken = default);
    void Remove(Sala sala);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
