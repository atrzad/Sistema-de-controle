using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Infrastructure.Data;

namespace SistemaDeControle.Infrastructure.Repositories;

public class JustificativaRepository : IJustificativaRepository
{
    private readonly AppDbContext _context;

    public JustificativaRepository(AppDbContext context) => _context = context;

    private IQueryable<Justificativa> ComIncludes() =>
        _context.Justificativas
            .Include(j => j.Anexos)
            .Include(j => j.RegistroFrequencia).ThenInclude(r => r.Professor);

    public Task<Justificativa?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        ComIncludes().FirstOrDefaultAsync(j => j.Id == id, cancellationToken);

    public Task<Justificativa?> GetByRegistroFrequenciaIdAsync(int registroFrequenciaId, CancellationToken cancellationToken = default) =>
        ComIncludes().FirstOrDefaultAsync(j => j.RegistroFrequenciaId == registroFrequenciaId, cancellationToken);

    public async Task<List<Justificativa>> ListAsync(
        int? professorId, DateOnly? dataInicio, DateOnly? dataFim,
        CancellationToken cancellationToken = default)
    {
        var query = ComIncludes();

        if (professorId.HasValue)
            query = query.Where(j => j.RegistroFrequencia.ProfessorId == professorId.Value);

        if (dataInicio.HasValue)
            query = query.Where(j => j.RegistroFrequencia.Data >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(j => j.RegistroFrequencia.Data <= dataFim.Value);

        return await query.OrderByDescending(j => j.DataEnvio).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Justificativa justificativa, CancellationToken cancellationToken = default) =>
        await _context.Justificativas.AddAsync(justificativa, cancellationToken);

    public void Remove(Justificativa justificativa) => _context.Justificativas.Remove(justificativa);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
