using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.DTOs.Cronograma;

public record GradeDiaDto(DiaSemana DiaSemana, List<AulaAgendadaResponseDto> Aulas);
