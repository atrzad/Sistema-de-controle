using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;
using SistemaDeControle.Infrastructure.Data;

namespace SistemaDeControle.Infrastructure.Repositories;

public class FrequenciaRepository : IFrequenciaRepository
{
    private readonly AppDbContext _context;

    public FrequenciaRepository(AppDbContext context) => _context = context;

    private IQueryable<RegistroFrequencia> ComIncludes() =>
        _context.RegistrosFrequencia
            .Include(r => r.Professor)
            .Include(r => r.AulaAgendada).ThenInclude(a => a.Turma)
            .Include(r => r.AulaAgendada).ThenInclude(a => a.Sala)
            .Include(r => r.Justificativa);

    public Task<RegistroFrequencia?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        ComIncludes().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<RegistroFrequencia?> GetByAulaEDataAsync(int aulaAgendadaId, DateOnly data, CancellationToken cancellationToken = default) =>
        ComIncludes().FirstOrDefaultAsync(r => r.AulaAgendadaId == aulaAgendadaId && r.Data == data, cancellationToken);

    public async Task<List<RegistroFrequencia>> ListAsync(
        int? professorId, DateOnly? dataInicio, DateOnly? dataFim, StatusFrequencia? status,
        CancellationToken cancellationToken = default)
    {
        var query = ComIncludes();

        if (professorId.HasValue) query = query.Where(r => r.ProfessorId == professorId.Value);
        if (dataInicio.HasValue) query = query.Where(r => r.Data >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(r => r.Data <= dataFim.Value);
        if (status.HasValue) query = query.Where(r => r.Status == status.Value);

        return await query.OrderByDescending(r => r.Data).ToListAsync(cancellationToken);
    }

    public async Task<List<RegistroFrequencia>> ListByDataAsync(DateOnly data, CancellationToken cancellationToken = default) =>
        await ComIncludes().Where(r => r.Data == data).ToListAsync(cancellationToken);

    public async Task AddAsync(RegistroFrequencia registro, CancellationToken cancellationToken = default) =>
        await _context.RegistrosFrequencia.AddAsync(registro, cancellationToken);

    public void Remove(RegistroFrequencia registro) => _context.RegistrosFrequencia.Remove(registro);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
