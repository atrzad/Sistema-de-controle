using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Infrastructure.Data;

namespace SistemaDeControle.Infrastructure.Repositories;

public class ProfessorRepository : IProfessorRepository
{
    private readonly AppDbContext _context;

    public ProfessorRepository(AppDbContext context) => _context = context;

    public Task<Professor?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Professores.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<List<Professor>> ListAsync(string? nome, bool? ativo, CancellationToken cancellationToken = default)
    {
        var query = _context.Professores.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(p => EF.Functions.ILike(p.Nome, $"%{nome}%"));

        if (ativo.HasValue)
            query = query.Where(p => p.Ativo == ativo.Value);

        return await query.OrderBy(p => p.Nome).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default) =>
        _context.Professores.AnyAsync(p => p.Email == email && (!excludeId.HasValue || p.Id != excludeId.Value), cancellationToken);

    public Task<bool> ExistsByMatriculaAsync(string matricula, int? excludeId = null, CancellationToken cancellationToken = default) =>
        _context.Professores.AnyAsync(p => p.Matricula == matricula && (!excludeId.HasValue || p.Id != excludeId.Value), cancellationToken);

    public async Task AddAsync(Professor professor, CancellationToken cancellationToken = default) =>
        await _context.Professores.AddAsync(professor, cancellationToken);

    public void Remove(Professor professor) => _context.Professores.Remove(professor);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
