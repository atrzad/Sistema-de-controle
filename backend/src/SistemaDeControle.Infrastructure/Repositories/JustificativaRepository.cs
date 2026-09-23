using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Application.Common;
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

    public async Task<ResultadoPaginado<Justificativa>> ListAsync(
        int? professorId, DateOnly? dataInicio, DateOnly? dataFim, Paginacao? paginacao,
        CancellationToken cancellationToken = default)
    {
        var query = ComIncludes().AsNoTracking();

        if (professorId.HasValue)
            query = query.Where(j => j.RegistroFrequencia.ProfessorId == professorId.Value);

        if (dataInicio.HasValue)
            query = query.Where(j => j.RegistroFrequencia.Data >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(j => j.RegistroFrequencia.Data <= dataFim.Value);

        var ordenada = query.OrderByDescending(j => j.DataEnvio).ThenBy(j => j.Id);

        if (paginacao is null)
        {
            var todas = await ordenada.ToListAsync(cancellationToken);
            return new ResultadoPaginado<Justificativa>(todas, todas.Count);
        }

        var total = await ordenada.CountAsync(cancellationToken);
        var itens = await ordenada.Skip(paginacao.Skip).Take(paginacao.TamanhoPagina).ToListAsync(cancellationToken);
        return new ResultadoPaginado<Justificativa>(itens, total);
    }

    public async Task AddAsync(Justificativa justificativa, CancellationToken cancellationToken = default) =>
        await _context.Justificativas.AddAsync(justificativa, cancellationToken);

    public void Remove(Justificativa justificativa) => _context.Justificativas.Remove(justificativa);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
