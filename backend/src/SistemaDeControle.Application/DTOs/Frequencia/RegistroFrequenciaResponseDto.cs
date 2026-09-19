using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.DTOs.Frequencia;

public record RegistroFrequenciaResponseDto(
    int Id,
    int AulaAgendadaId,
    int ProfessorId,
    string ProfessorNome,
    string TurmaNome,
    string SalaNome,
    DiaSemana DiaSemana,
    TimeOnly HoraInicio,
    TimeOnly HoraFim,
    DateOnly Data,
    StatusFrequencia Status,
    string? Observacao,
    int RegistradoPorUsuarioId,
    bool TemJustificativa);
