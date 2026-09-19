using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
