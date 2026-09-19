using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Infrastructure.Data;

namespace SistemaDeControle.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context) => _context = context;

    public Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
}
