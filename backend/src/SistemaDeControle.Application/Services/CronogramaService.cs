using SistemaDeControle.Application.Common.Exceptions;
using SistemaDeControle.Application.DTOs.Cronograma;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.Services;

public class CronogramaService : ICronogramaService
{
    private readonly ICronogramaRepository _cronogramaRepository;
    private readonly IProfessorRepository _professorRepository;
    private readonly ITurmaRepository _turmaRepository;
    private readonly ISalaRepository _salaRepository;

    public CronogramaService(
        ICronogramaRepository cronogramaRepository,
        IProfessorRepository professorRepository,
        ITurmaRepository turmaRepository,
        ISalaRepository salaRepository)
    {
        _cronogramaRepository = cronogramaRepository;
        _professorRepository = professorRepository;
        _turmaRepository = turmaRepository;
        _salaRepository = salaRepository;
    }

    public async Task<List<AulaAgendadaResponseDto>> ListAsync(int? professorId, int? turmaId, int? salaId, DiaSemana? diaSemana, CancellationToken cancellationToken = default)
    {
        var aulas = await _cronogramaRepository.ListAsync(professorId, turmaId, salaId, diaSemana, cancellationToken);
        return aulas.Select(MapToDto).ToList();
    }

    public async Task<AulaAgendadaResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var aula = await _cronogramaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(AulaAgendada), id);

        return MapToDto(aula);
    }

    public async Task<AulaAgendadaResponseDto> CreateAsync(AulaAgendadaCreateDto dto, CancellationToken cancellationToken = default)
    {
        var (professor, turma, sala) = await ValidarReferenciasAsync(dto.ProfessorId, dto.TurmaId, dto.SalaId, cancellationToken);
        await ValidarConflitosAsync(dto.SalaId, dto.ProfessorId, dto.DiaSemana, dto.HoraInicio, dto.HoraFim, excludeId: null, cancellationToken);

        var aula = new AulaAgendada
        {
            ProfessorId = dto.ProfessorId,
            Professor = professor,
            TurmaId = dto.TurmaId,
            Turma = turma,
            SalaId = dto.SalaId,
            Sala = sala,
            DiaSemana = dto.DiaSemana,
            HoraInicio = dto.HoraInicio,
            HoraFim = dto.HoraFim,
            Disciplina = dto.Disciplina,
            VigenteDesde = dto.VigenteDesde,
            VigenteAte = dto.VigenteAte,
            Ativo = true,
        };

        await _cronogramaRepository.AddAsync(aula, cancellationToken);
        await _cronogramaRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(aula);
    }

    public async Task<AulaAgendadaResponseDto> UpdateAsync(int id, AulaAgendadaUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var aula = await _cronogramaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(AulaAgendada), id);

        var (professor, turma, sala) = await ValidarReferenciasAsync(dto.ProfessorId, dto.TurmaId, dto.SalaId, cancellationToken);

        if (dto.Ativo)
        {
            await ValidarConflitosAsync(dto.SalaId, dto.ProfessorId, dto.DiaSemana, dto.HoraInicio, dto.HoraFim, excludeId: id, cancellationToken);
        }

        aula.ProfessorId = dto.ProfessorId;
        aula.Professor = professor;
        aula.TurmaId = dto.TurmaId;
        aula.Turma = turma;
        aula.SalaId = dto.SalaId;
        aula.Sala = sala;
        aula.DiaSemana = dto.DiaSemana;
        aula.HoraInicio = dto.HoraInicio;
        aula.HoraFim = dto.HoraFim;
        aula.Disciplina = dto.Disciplina;
        aula.VigenteDesde = dto.VigenteDesde;
        aula.VigenteAte = dto.VigenteAte;
        aula.Ativo = dto.Ativo;

        await _cronogramaRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(aula);
    }

    public async Task InativarAsync(int id, CancellationToken cancellationToken = default)
    {
        var aula = await _cronogramaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(AulaAgendada), id);

        aula.Ativo = false;
        await _cronogramaRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<GradeDiaDto>> GetGradeSemanalAsync(CancellationToken cancellationToken = default)
    {
        var aulas = await _cronogramaRepository.GetGradeSemanalAsync(cancellationToken);

        return aulas
            .GroupBy(a => a.DiaSemana)
            .OrderBy(g => g.Key)
            .Select(g => new GradeDiaDto(g.Key, g.OrderBy(a => a.HoraInicio).Select(MapToDto).ToList()))
            .ToList();
    }

    public async Task<List<AulaAgendadaResponseDto>> GetCronogramaDoProfessorAsync(int professorId, CancellationToken cancellationToken = default)
    {
        _ = await _professorRepository.GetByIdAsync(professorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Professor), professorId);

        var aulas = await _cronogramaRepository.ListAsync(professorId, null, null, null, cancellationToken);
        return aulas.Select(MapToDto).ToList();
    }

    public async Task<List<AulaAgendadaResponseDto>> GetCronogramaDaTurmaAsync(int turmaId, CancellationToken cancellationToken = default)
    {
        _ = await _turmaRepository.GetByIdAsync(turmaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Turma), turmaId);

        var aulas = await _cronogramaRepository.ListAsync(null, turmaId, null, null, cancellationToken);
        return aulas.Select(MapToDto).ToList();
    }

    private async Task<(Professor Professor, Turma Turma, Sala Sala)> ValidarReferenciasAsync(
        int professorId, int turmaId, int salaId, CancellationToken cancellationToken)
    {
        var professor = await _professorRepository.GetByIdAsync(professorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Professor), professorId);

        var turma = await _turmaRepository.GetByIdAsync(turmaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Turma), turmaId);

        var sala = await _salaRepository.GetByIdAsync(salaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Sala), salaId);

        return (professor, turma, sala);
    }

    private async Task ValidarConflitosAsync(
        int salaId, int professorId, DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFim,
        int? excludeId, CancellationToken cancellationToken)
    {
        var aulasNaSala = await _cronogramaRepository.GetPorSalaEDiaAsync(salaId, diaSemana, excludeId, cancellationToken);
        if (aulasNaSala.Any(a => Sobrepoe(a.HoraInicio, a.HoraFim, horaInicio, horaFim)))
            throw new ConflictException("Já existe uma aula agendada nessa sala, nesse dia e horário.");

        var aulasDoProfessor = await _cronogramaRepository.GetPorProfessorEDiaAsync(professorId, diaSemana, excludeId, cancellationToken);
        if (aulasDoProfessor.Any(a => Sobrepoe(a.HoraInicio, a.HoraFim, horaInicio, horaFim)))
            throw new ConflictException("O professor já possui aula agendada nesse dia e horário.");
    }

    private static bool Sobrepoe(TimeOnly inicioA, TimeOnly fimA, TimeOnly inicioB, TimeOnly fimB) =>
        inicioA < fimB && inicioB < fimA;

    private static AulaAgendadaResponseDto MapToDto(AulaAgendada aula) => new(
        aula.Id,
        aula.ProfessorId, aula.Professor.Nome,
        aula.TurmaId, aula.Turma.Nome,
        aula.SalaId, aula.Sala.Nome,
        aula.DiaSemana, aula.HoraInicio, aula.HoraFim,
        aula.Disciplina, aula.VigenteDesde, aula.VigenteAte, aula.Ativo);
}
