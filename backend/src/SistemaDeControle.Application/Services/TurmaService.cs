using SistemaDeControle.Application.Common.Exceptions;
using SistemaDeControle.Application.DTOs.Professores;
using SistemaDeControle.Application.DTOs.Turmas;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.Services;

public class TurmaService : ITurmaService
{
    private readonly ITurmaRepository _turmaRepository;
    private readonly ICronogramaRepository _cronogramaRepository;

    public TurmaService(ITurmaRepository turmaRepository, ICronogramaRepository cronogramaRepository)
    {
        _turmaRepository = turmaRepository;
        _cronogramaRepository = cronogramaRepository;
    }

    public async Task<List<TurmaResponseDto>> ListAsync(int? anoLetivo, Turno? turno, bool? ativo, CancellationToken cancellationToken = default)
    {
        var turmas = await _turmaRepository.ListAsync(anoLetivo, turno, ativo, cancellationToken);
        return turmas.Select(MapToDto).ToList();
    }

    public async Task<TurmaResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var turma = await _turmaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Turma), id);

        return MapToDto(turma);
    }

    public async Task<TurmaResponseDto> CreateAsync(TurmaCreateDto dto, CancellationToken cancellationToken = default)
    {
        var turma = new Turma
        {
            Nome = dto.Nome,
            Turno = dto.Turno,
            AnoLetivo = dto.AnoLetivo,
            Ativo = true,
        };

        await _turmaRepository.AddAsync(turma, cancellationToken);
        await _turmaRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(turma);
    }

    public async Task<TurmaResponseDto> UpdateAsync(int id, TurmaUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var turma = await _turmaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Turma), id);

        turma.Nome = dto.Nome;
        turma.Turno = dto.Turno;
        turma.AnoLetivo = dto.AnoLetivo;
        turma.Ativo = dto.Ativo;

        await _turmaRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(turma);
    }

    public async Task InativarAsync(int id, CancellationToken cancellationToken = default)
    {
        var turma = await _turmaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Turma), id);

        turma.Ativo = false;
        await _turmaRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ProfessorResponseDto>> GetProfessoresDaTurmaAsync(int turmaId, CancellationToken cancellationToken = default)
    {
        _ = await _turmaRepository.GetByIdAsync(turmaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Turma), turmaId);

        var aulas = await _cronogramaRepository.ListAsync(null, turmaId, null, null, cancellationToken);

        return aulas
            .Select(a => a.Professor)
            .DistinctBy(p => p.Id)
            .OrderBy(p => p.Nome)
            .Select(p => new ProfessorResponseDto(p.Id, p.Nome, p.Email, p.Telefone, p.Matricula, p.Disciplina, p.Ativo, p.CreatedAt))
            .ToList();
    }

    private static TurmaResponseDto MapToDto(Turma turma) => new(turma.Id, turma.Nome, turma.Turno, turma.AnoLetivo, turma.Ativo);
}
