using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;
using SistemaDeControle.Infrastructure.Data;

namespace SistemaDeControle.Infrastructure.Repositories;

public class CronogramaRepository : ICronogramaRepository
{
    private readonly AppDbContext _context;

    public CronogramaRepository(AppDbContext context) => _context = context;

    private IQueryable<AulaAgendada> ComIncludes() =>
        _context.AulasAgendadas
            .Include(a => a.Professor)
            .Include(a => a.Turma)
            .Include(a => a.Sala);

    public Task<AulaAgendada?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        ComIncludes().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<List<AulaAgendada>> ListAsync(
        int? professorId, int? turmaId, int? salaId, DiaSemana? diaSemana,
        CancellationToken cancellationToken = default)
    {
        var query = ComIncludes();

        if (professorId.HasValue) query = query.Where(a => a.ProfessorId == professorId.Value);
        if (turmaId.HasValue) query = query.Where(a => a.TurmaId == turmaId.Value);
        if (salaId.HasValue) query = query.Where(a => a.SalaId == salaId.Value);
        if (diaSemana.HasValue) query = query.Where(a => a.DiaSemana == diaSemana.Value);

        return await query
            .OrderBy(a => a.DiaSemana).ThenBy(a => a.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AulaAgendada>> GetPorSalaEDiaAsync(int salaId, DiaSemana diaSemana, int? excludeId = null, CancellationToken cancellationToken = default) =>
        await _context.AulasAgendadas
            .Where(a => a.SalaId == salaId && a.DiaSemana == diaSemana && a.Ativo)
            .Where(a => !excludeId.HasValue || a.Id != excludeId.Value)
            .ToListAsync(cancellationToken);

    public async Task<List<AulaAgendada>> GetPorProfessorEDiaAsync(int professorId, DiaSemana diaSemana, int? excludeId = null, CancellationToken cancellationToken = default) =>
        await _context.AulasAgendadas
            .Where(a => a.ProfessorId == professorId && a.DiaSemana == diaSemana && a.Ativo)
            .Where(a => !excludeId.HasValue || a.Id != excludeId.Value)
            .ToListAsync(cancellationToken);

    public async Task<List<AulaAgendada>> GetGradeSemanalAsync(CancellationToken cancellationToken = default) =>
        await ComIncludes()
            .Where(a => a.Ativo)
            .OrderBy(a => a.DiaSemana).ThenBy(a => a.HoraInicio)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(AulaAgendada aula, CancellationToken cancellationToken = default) =>
        await _context.AulasAgendadas.AddAsync(aula, cancellationToken);

    public void Remove(AulaAgendada aula) => _context.AulasAgendadas.Remove(aula);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
