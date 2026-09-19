using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.DTOs.Cronograma;

public record AulaAgendadaResponseDto(
    int Id,
    int ProfessorId,
    string ProfessorNome,
    int TurmaId,
    string TurmaNome,
    int SalaId,
    string SalaNome,
    DiaSemana DiaSemana,
    TimeOnly HoraInicio,
    TimeOnly HoraFim,
    string? Disciplina,
    DateOnly? VigenteDesde,
    DateOnly? VigenteAte,
    bool Ativo);
