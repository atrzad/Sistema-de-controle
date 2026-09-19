using SistemaDeControle.Application.DTOs.Professores;
using SistemaDeControle.Application.DTOs.Turmas;

namespace SistemaDeControle.Application.Interfaces;

public interface IProfessorService
{
    Task<List<ProfessorResponseDto>> ListAsync(string? nome, bool? ativo, CancellationToken cancellationToken = default);
    Task<ProfessorResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProfessorResponseDto> CreateAsync(ProfessorCreateDto dto, CancellationToken cancellationToken = default);
    Task<ProfessorResponseDto> UpdateAsync(int id, ProfessorUpdateDto dto, CancellationToken cancellationToken = default);
    Task InativarAsync(int id, CancellationToken cancellationToken = default);
    Task<List<TurmaResponseDto>> GetTurmasDoProfessorAsync(int professorId, CancellationToken cancellationToken = default);
}
