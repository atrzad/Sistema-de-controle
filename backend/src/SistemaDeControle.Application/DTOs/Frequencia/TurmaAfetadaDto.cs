using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.DTOs.Frequencia;

public record TurmaAfetadaDto(
    DateOnly Data,
    int ProfessorId,
    string ProfessorNome,
    int TurmaId,
    string TurmaNome,
    int SalaId,
    string SalaNome,
    TimeOnly HoraInicio,
    TimeOnly HoraFim,
    StatusFrequencia Status);
