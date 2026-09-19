using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.DTOs.Frequencia;

/// <summary>Uma aula prevista pelo cronograma para uma data específica, com o status de frequência já registrado (se houver).</summary>
public record OcorrenciaDiaDto(
    int AulaAgendadaId,
    int ProfessorId,
    string ProfessorNome,
    int TurmaId,
    string TurmaNome,
    int SalaId,
    string SalaNome,
    TimeOnly HoraInicio,
    TimeOnly HoraFim,
    string? Disciplina,
    int? RegistroFrequenciaId,
    StatusFrequencia? Status);
