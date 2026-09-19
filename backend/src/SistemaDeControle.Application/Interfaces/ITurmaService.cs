using SistemaDeControle.Application.DTOs.Professores;
using SistemaDeControle.Application.DTOs.Turmas;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.Interfaces;

public interface ITurmaService
{
    Task<List<TurmaResponseDto>> ListAsync(int? anoLetivo, Turno? turno, bool? ativo, CancellationToken cancellationToken = default);
    Task<TurmaResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TurmaResponseDto> CreateAsync(TurmaCreateDto dto, CancellationToken cancellationToken = default);
    Task<TurmaResponseDto> UpdateAsync(int id, TurmaUpdateDto dto, CancellationToken cancellationToken = default);
    Task InativarAsync(int id, CancellationToken cancellationToken = default);
    Task<List<ProfessorResponseDto>> GetProfessoresDaTurmaAsync(int turmaId, CancellationToken cancellationToken = default);
}
