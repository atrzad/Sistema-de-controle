using SistemaDeControle.Application.Common.Exceptions;
using SistemaDeControle.Application.DTOs.Frequencia;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.Services;

public class FrequenciaService : IFrequenciaService
{
    private readonly IFrequenciaRepository _frequenciaRepository;
    private readonly ICronogramaRepository _cronogramaRepository;

    public FrequenciaService(IFrequenciaRepository frequenciaRepository, ICronogramaRepository cronogramaRepository)
    {
        _frequenciaRepository = frequenciaRepository;
        _cronogramaRepository = cronogramaRepository;
    }

    public async Task<List<RegistroFrequenciaResponseDto>> ListAsync(int? professorId, DateOnly? dataInicio, DateOnly? dataFim, StatusFrequencia? status, CancellationToken cancellationToken = default)
    {
        var registros = await _frequenciaRepository.ListAsync(professorId, dataInicio, dataFim, status, cancellationToken);
        return registros.Select(MapToDto).ToList();
    }

    public async Task<RegistroFrequenciaResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var registro = await _frequenciaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(RegistroFrequencia), id);

        return MapToDto(registro);
    }

    public async Task<RegistroFrequenciaResponseDto> RegistrarAsync(RegistroFrequenciaCreateDto dto, int usuarioId, CancellationToken cancellationToken = default)
    {
        var aula = await _cronogramaRepository.GetByIdAsync(dto.AulaAgendadaId, cancellationToken)
            ?? throw new NotFoundException(nameof(AulaAgendada), dto.AulaAgendadaId);

        var existente = await _frequenciaRepository.GetByAulaEDataAsync(dto.AulaAgendadaId, dto.Data, cancellationToken);
        if (existente is not null)
            throw new ConflictException("Já existe um registro de frequência para esta aula nesta data.");

        var registro = new RegistroFrequencia
        {
            AulaAgendadaId = dto.AulaAgendadaId,
            AulaAgendada = aula,
            ProfessorId = aula.ProfessorId,
            Professor = aula.Professor,
            Data = dto.Data,
            Status = dto.Status,
            Observacao = dto.Observacao,
            RegistradoPorUsuarioId = usuarioId,
        };

        await _frequenciaRepository.AddAsync(registro, cancellationToken);
        await _frequenciaRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(registro);
    }

    public async Task<RegistroFrequenciaResponseDto> AtualizarAsync(int id, RegistroFrequenciaUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var registro = await _frequenciaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(RegistroFrequencia), id);

        registro.Status = dto.Status;
        registro.Observacao = dto.Observacao;

        await _frequenciaRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(registro);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var registro = await _frequenciaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(RegistroFrequencia), id);

        _frequenciaRepository.Remove(registro);
        await _frequenciaRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<OcorrenciaDiaDto>> GetOcorrenciasDoDiaAsync(DateOnly data, CancellationToken cancellationToken = default)
    {
        var diaSemana = (DiaSemana)(int)data.DayOfWeek;

        var aulasDoDia = (await _cronogramaRepository.ListAsync(null, null, null, diaSemana, cancellationToken))
            .Where(a => a.Ativo)
            .Where(a => !a.VigenteDesde.HasValue || a.VigenteDesde.Value <= data)
            .Where(a => !a.VigenteAte.HasValue || a.VigenteAte.Value >= data)
            .ToList();

        var registrosDoDia = await _frequenciaRepository.ListByDataAsync(data, cancellationToken);
        var registrosPorAula = registrosDoDia.ToDictionary(r => r.AulaAgendadaId);

        return aulasDoDia
            .OrderBy(a => a.HoraInicio)
            .Select(a =>
            {
                registrosPorAula.TryGetValue(a.Id, out var registro);
                return new OcorrenciaDiaDto(
                    a.Id, a.ProfessorId, a.Professor.Nome,
                    a.TurmaId, a.Turma.Nome, a.SalaId, a.Sala.Nome,
                    a.HoraInicio, a.HoraFim, a.Disciplina,
                    registro?.Id, registro?.Status);
            })
            .ToList();
    }

    public async Task<List<TurmaAfetadaDto>> GetTurmasAfetadasAsync(DateOnly data, int? professorId, CancellationToken cancellationToken = default)
    {
        var registros = await _frequenciaRepository.ListByDataAsync(data, cancellationToken);

        return registros
            .Where(r => r.Status is StatusFrequencia.Ausente or StatusFrequencia.AusenciaJustificada)
            .Where(r => !professorId.HasValue || r.ProfessorId == professorId.Value)
            .OrderBy(r => r.AulaAgendada.HoraInicio)
            .Select(r => new TurmaAfetadaDto(
                r.Data, r.ProfessorId, r.Professor.Nome,
                r.AulaAgendada.TurmaId, r.AulaAgendada.Turma.Nome,
                r.AulaAgendada.SalaId, r.AulaAgendada.Sala.Nome,
                r.AulaAgendada.HoraInicio, r.AulaAgendada.HoraFim, r.Status))
            .ToList();
    }

    private static RegistroFrequenciaResponseDto MapToDto(RegistroFrequencia registro) => new(
        registro.Id, registro.AulaAgendadaId, registro.ProfessorId, registro.Professor.Nome,
        registro.AulaAgendada.Turma.Nome, registro.AulaAgendada.Sala.Nome, registro.AulaAgendada.DiaSemana,
        registro.AulaAgendada.HoraInicio, registro.AulaAgendada.HoraFim,
        registro.Data, registro.Status, registro.Observacao, registro.RegistradoPorUsuarioId,
        registro.Justificativa is not null);
}
