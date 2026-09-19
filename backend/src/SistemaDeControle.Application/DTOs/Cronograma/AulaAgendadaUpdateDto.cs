using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.DTOs.Cronograma;

public record AulaAgendadaUpdateDto(
    int ProfessorId,
    int TurmaId,
    int SalaId,
    DiaSemana DiaSemana,
    TimeOnly HoraInicio,
    TimeOnly HoraFim,
    string? Disciplina,
    DateOnly? VigenteDesde,
    DateOnly? VigenteAte,
    bool Ativo);
