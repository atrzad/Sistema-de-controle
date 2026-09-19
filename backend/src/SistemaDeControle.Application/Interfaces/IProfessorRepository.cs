using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Application.Interfaces;

public interface IProfessorRepository
{
    Task<Professor?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Professor>> ListAsync(string? nome, bool? ativo, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByMatriculaAsync(string matricula, int? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Professor professor, CancellationToken cancellationToken = default);
    void Remove(Professor professor);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
