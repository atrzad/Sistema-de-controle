using SistemaDeControle.Application.Common.Exceptions;
using SistemaDeControle.Application.DTOs.Frequencia;
using SistemaDeControle.Application.DTOs.Relatorios;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.Services;

public class RelatorioService : IRelatorioService
{
    private readonly IFrequenciaRepository _frequenciaRepository;
    private readonly IProfessorRepository _professorRepository;

    public RelatorioService(IFrequenciaRepository frequenciaRepository, IProfessorRepository professorRepository)
    {
        _frequenciaRepository = frequenciaRepository;
        _professorRepository = professorRepository;
    }

    public async Task<RelatorioFrequenciaDto> GetRelatorioSemanalAsync(int professorId, DateOnly dataReferencia, CancellationToken cancellationToken = default)
    {
        var diasDesdeSegunda = ((int)dataReferencia.DayOfWeek + 6) % 7;
        var inicio = dataReferencia.AddDays(-diasDesdeSegunda);
        var fim = inicio.AddDays(6);

        return await MontarRelatorioAsync(professorId, inicio, fim, cancellationToken);
    }

    public async Task<RelatorioFrequenciaDto> GetRelatorioMensalAsync(int professorId, int mes, int ano, CancellationToken cancellationToken = default)
    {
        var inicio = new DateOnly(ano, mes, 1);
        var fim = inicio.AddMonths(1).AddDays(-1);

        return await MontarRelatorioAsync(professorId, inicio, fim, cancellationToken);
    }

    public async Task<List<RelatorioFrequenciaDto>> GetRelatorioConsolidadoAsync(DateOnly dataInicio, DateOnly dataFim, CancellationToken cancellationToken = default)
    {
        var professores = await _professorRepository.ListAsync(null, true, cancellationToken);

        var relatorios = new List<RelatorioFrequenciaDto>();
        foreach (var professor in professores)
        {
            var registros = await _frequenciaRepository.ListAsync(professor.Id, dataInicio, dataFim, null, cancellationToken);
            relatorios.Add(CalcularRelatorio(professor.Id, professor.Nome, dataInicio, dataFim, registros));
        }

        return relatorios;
    }

    public async Task<List<TurmaAfetadaDto>> GetTurmasAfetadasHistoricoAsync(DateOnly dataInicio, DateOnly dataFim, CancellationToken cancellationToken = default)
    {
        var registros = await _frequenciaRepository.ListAsync(null, dataInicio, dataFim, null, cancellationToken);

        return registros
            .Where(r => r.Status is StatusFrequencia.Ausente or StatusFrequencia.AusenciaJustificada)
            .OrderBy(r => r.Data).ThenBy(r => r.AulaAgendada.HoraInicio)
            .Select(r => new TurmaAfetadaDto(
                r.Data, r.ProfessorId, r.Professor.Nome,
                r.AulaAgendada.TurmaId, r.AulaAgendada.Turma.Nome,
                r.AulaAgendada.SalaId, r.AulaAgendada.Sala.Nome,
                r.AulaAgendada.HoraInicio, r.AulaAgendada.HoraFim, r.Status))
            .ToList();
    }

    private async Task<RelatorioFrequenciaDto> MontarRelatorioAsync(int professorId, DateOnly inicio, DateOnly fim, CancellationToken cancellationToken)
    {
        var professor = await _professorRepository.GetByIdAsync(professorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Professor), professorId);

        var registros = await _frequenciaRepository.ListAsync(professorId, inicio, fim, null, cancellationToken);

        return CalcularRelatorio(professor.Id, professor.Nome, inicio, fim, registros);
    }

    private static RelatorioFrequenciaDto CalcularRelatorio(int professorId, string professorNome, DateOnly inicio, DateOnly fim, List<RegistroFrequencia> registros)
    {
        var totalAulas = registros.Count;
        var totalPresencas = registros.Count(r => r.Status == StatusFrequencia.Presente);
        var totalAusencias = registros.Count(r => r.Status == StatusFrequencia.Ausente);
        var totalAusenciasJustificadas = registros.Count(r => r.Status == StatusFrequencia.AusenciaJustificada);
        var percentualPresenca = totalAulas == 0 ? 0 : Math.Round(totalPresencas * 100.0 / totalAulas, 2);

        return new RelatorioFrequenciaDto(professorId, professorNome, inicio, fim, totalAulas, totalPresencas, totalAusencias, totalAusenciasJustificadas, percentualPresenca);
    }
}
