using SistemaDeControle.Application.Common.Exceptions;
using SistemaDeControle.Application.DTOs.Professores;
using SistemaDeControle.Application.DTOs.Turmas;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Application.Services;

public class ProfessorService : IProfessorService
{
    private readonly IProfessorRepository _professorRepository;
    private readonly ICronogramaRepository _cronogramaRepository;

    public ProfessorService(IProfessorRepository professorRepository, ICronogramaRepository cronogramaRepository)
    {
        _professorRepository = professorRepository;
        _cronogramaRepository = cronogramaRepository;
    }

    public async Task<List<ProfessorResponseDto>> ListAsync(string? nome, bool? ativo, CancellationToken cancellationToken = default)
    {
        var professores = await _professorRepository.ListAsync(nome, ativo, cancellationToken);
        return professores.Select(MapToDto).ToList();
    }

    public async Task<ProfessorResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var professor = await _professorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Professor), id);

        return MapToDto(professor);
    }

    public async Task<ProfessorResponseDto> CreateAsync(ProfessorCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (await _professorRepository.ExistsByEmailAsync(dto.Email, cancellationToken: cancellationToken))
            throw new ConflictException($"Já existe um professor com o e-mail '{dto.Email}'.");

        if (await _professorRepository.ExistsByMatriculaAsync(dto.Matricula, cancellationToken: cancellationToken))
            throw new ConflictException($"Já existe um professor com a matrícula '{dto.Matricula}'.");

        var professor = new Professor
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            Matricula = dto.Matricula,
            Disciplina = dto.Disciplina,
            Ativo = true,
        };

        await _professorRepository.AddAsync(professor, cancellationToken);
        await _professorRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(professor);
    }

    public async Task<ProfessorResponseDto> UpdateAsync(int id, ProfessorUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var professor = await _professorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Professor), id);

        if (await _professorRepository.ExistsByEmailAsync(dto.Email, id, cancellationToken))
            throw new ConflictException($"Já existe um professor com o e-mail '{dto.Email}'.");

        if (await _professorRepository.ExistsByMatriculaAsync(dto.Matricula, id, cancellationToken))
            throw new ConflictException($"Já existe um professor com a matrícula '{dto.Matricula}'.");

        professor.Nome = dto.Nome;
        professor.Email = dto.Email;
        professor.Telefone = dto.Telefone;
        professor.Matricula = dto.Matricula;
        professor.Disciplina = dto.Disciplina;
        professor.Ativo = dto.Ativo;

        await _professorRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(professor);
    }

    public async Task InativarAsync(int id, CancellationToken cancellationToken = default)
    {
        var professor = await _professorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Professor), id);

        professor.Ativo = false;
        await _professorRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<TurmaResponseDto>> GetTurmasDoProfessorAsync(int professorId, CancellationToken cancellationToken = default)
    {
        _ = await _professorRepository.GetByIdAsync(professorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Professor), professorId);

        var aulas = await _cronogramaRepository.ListAsync(professorId, null, null, null, cancellationToken);

        return aulas
            .Select(a => a.Turma)
            .DistinctBy(t => t.Id)
            .OrderBy(t => t.Nome)
            .Select(t => new TurmaResponseDto(t.Id, t.Nome, t.Turno, t.AnoLetivo, t.Ativo))
            .ToList();
    }

    private static ProfessorResponseDto MapToDto(Professor professor) => new(
        professor.Id, professor.Nome, professor.Email, professor.Telefone,
        professor.Matricula, professor.Disciplina, professor.Ativo, professor.CreatedAt);
}
