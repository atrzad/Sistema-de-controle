using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.DTOs.Turmas;

public record TurmaUpdateDto(string Nome, Turno Turno, int AnoLetivo, bool Ativo);
