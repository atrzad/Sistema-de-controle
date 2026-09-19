using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.Interfaces;

public interface ICronogramaRepository
{
    Task<AulaAgendada?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<AulaAgendada>> ListAsync(
        int? professorId, int? turmaId, int? salaId, DiaSemana? diaSemana,
        CancellationToken cancellationToken = default);

    /// <summary>Aulas ativas na mesma sala e dia da semana (candidatas a conflito de horário).</summary>
    Task<List<AulaAgendada>> GetPorSalaEDiaAsync(int salaId, DiaSemana diaSemana, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>Aulas ativas do mesmo professor e dia da semana (candidatas a conflito de horário).</summary>
    Task<List<AulaAgendada>> GetPorProfessorEDiaAsync(int professorId, DiaSemana diaSemana, int? excludeId = null, CancellationToken cancellationToken = default);

    Task<List<AulaAgendada>> GetGradeSemanalAsync(CancellationToken cancellationToken = default);

    Task AddAsync(AulaAgendada aula, CancellationToken cancellationToken = default);
    void Remove(AulaAgendada aula);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
