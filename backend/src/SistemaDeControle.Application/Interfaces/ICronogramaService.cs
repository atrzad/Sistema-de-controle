using SistemaDeControle.Application.DTOs.Cronograma;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.Interfaces;

public interface ICronogramaService
{
    Task<List<AulaAgendadaResponseDto>> ListAsync(int? professorId, int? turmaId, int? salaId, DiaSemana? diaSemana, CancellationToken cancellationToken = default);
    Task<AulaAgendadaResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AulaAgendadaResponseDto> CreateAsync(AulaAgendadaCreateDto dto, CancellationToken cancellationToken = default);
    Task<AulaAgendadaResponseDto> UpdateAsync(int id, AulaAgendadaUpdateDto dto, CancellationToken cancellationToken = default);
    Task InativarAsync(int id, CancellationToken cancellationToken = default);
    Task<List<GradeDiaDto>> GetGradeSemanalAsync(CancellationToken cancellationToken = default);
    Task<List<AulaAgendadaResponseDto>> GetCronogramaDoProfessorAsync(int professorId, CancellationToken cancellationToken = default);
    Task<List<AulaAgendadaResponseDto>> GetCronogramaDaTurmaAsync(int turmaId, CancellationToken cancellationToken = default);
}
