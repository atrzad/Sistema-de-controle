using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Infrastructure.Data;

namespace SistemaDeControle.Infrastructure.Repositories;

public class SalaRepository : ISalaRepository
{
    private readonly AppDbContext _context;

    public SalaRepository(AppDbContext context) => _context = context;

    public Task<Sala?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Salas.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<List<Sala>> ListAsync(bool? ativo, CancellationToken cancellationToken = default)
    {
        var query = _context.Salas.AsQueryable();

        if (ativo.HasValue)
            query = query.Where(s => s.Ativo == ativo.Value);

        return await query.OrderBy(s => s.Nome).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByNomeAsync(string nome, int? excludeId = null, CancellationToken cancellationToken = default) =>
        _context.Salas.AnyAsync(s => s.Nome == nome && (!excludeId.HasValue || s.Id != excludeId.Value), cancellationToken);

    public async Task AddAsync(Sala sala, CancellationToken cancellationToken = default) =>
        await _context.Salas.AddAsync(sala, cancellationToken);

    public void Remove(Sala sala) => _context.Salas.Remove(sala);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
