using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Application.Common;
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

    private IQueryable<RegistroFrequencia> Filtrar(
        IQueryable<RegistroFrequencia> query, int? professorId, DateOnly? dataInicio, DateOnly? dataFim, StatusFrequencia? status)
    {
        if (professorId.HasValue) query = query.Where(r => r.ProfessorId == professorId.Value);
        if (dataInicio.HasValue) query = query.Where(r => r.Data >= dataInicio.Value);
        if (dataFim.HasValue) query = query.Where(r => r.Data <= dataFim.Value);
        if (status.HasValue) query = query.Where(r => r.Status == status.Value);
        return query;
    }

    public async Task<List<RegistroFrequencia>> ListAsync(
        int? professorId, DateOnly? dataInicio, DateOnly? dataFim, StatusFrequencia? status,
        CancellationToken cancellationToken = default) =>
        await Filtrar(ComIncludes().AsNoTracking(), professorId, dataInicio, dataFim, status)
            .OrderByDescending(r => r.Data)
            .ToListAsync(cancellationToken);

    public async Task<ResultadoPaginado<RegistroFrequencia>> ListPaginadoAsync(
        int? professorId, DateOnly? dataInicio, DateOnly? dataFim, StatusFrequencia? status, Paginacao? paginacao,
        CancellationToken cancellationToken = default)
    {
        var query = Filtrar(ComIncludes().AsNoTracking(), professorId, dataInicio, dataFim, status)
            .OrderByDescending(r => r.Data).ThenBy(r => r.Id);

        if (paginacao is null)
        {
            var todos = await query.ToListAsync(cancellationToken);
            return new ResultadoPaginado<RegistroFrequencia>(todos, todos.Count);
        }

        var total = await query.CountAsync(cancellationToken);
        var itens = await query.Skip(paginacao.Skip).Take(paginacao.TamanhoPagina).ToListAsync(cancellationToken);
        return new ResultadoPaginado<RegistroFrequencia>(itens, total);
    }

    public async Task<List<ContagemFrequencia>> ContarPorProfessorEStatusAsync(
        int? professorId, DateOnly dataInicio, DateOnly dataFim,
        CancellationToken cancellationToken = default) =>
        await Filtrar(_context.RegistrosFrequencia.AsNoTracking(), professorId, dataInicio, dataFim, null)
            .GroupBy(r => new { r.ProfessorId, r.Status })
            .Select(g => new ContagemFrequencia(g.Key.ProfessorId, g.Key.Status, g.Count()))
            .ToListAsync(cancellationToken);

    public async Task<List<RegistroFrequencia>> ListByDataAsync(DateOnly data, CancellationToken cancellationToken = default) =>
        await ComIncludes().AsNoTracking().Where(r => r.Data == data).ToListAsync(cancellationToken);

    public async Task AddAsync(RegistroFrequencia registro, CancellationToken cancellationToken = default) =>
        await _context.RegistrosFrequencia.AddAsync(registro, cancellationToken);

    public void Remove(RegistroFrequencia registro) => _context.RegistrosFrequencia.Remove(registro);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
