using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;
using SistemaDeControle.Infrastructure.Data;

namespace SistemaDeControle.Infrastructure.Repositories;

public class TurmaRepository : ITurmaRepository
{
    private readonly AppDbContext _context;

    public TurmaRepository(AppDbContext context) => _context = context;

    public Task<Turma?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Turmas.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<List<Turma>> ListAsync(int? anoLetivo, Turno? turno, bool? ativo, CancellationToken cancellationToken = default)
    {
        var query = _context.Turmas.AsNoTracking();

        if (anoLetivo.HasValue)
            query = query.Where(t => t.AnoLetivo == anoLetivo.Value);

        if (turno.HasValue)
            query = query.Where(t => t.Turno == turno.Value);

        if (ativo.HasValue)
            query = query.Where(t => t.Ativo == ativo.Value);

        return await query.OrderBy(t => t.Nome).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Turma turma, CancellationToken cancellationToken = default) =>
        await _context.Turmas.AddAsync(turma, cancellationToken);

    public void Remove(Turma turma) => _context.Turmas.Remove(turma);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
