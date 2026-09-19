using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.DTOs.Turmas;

public record TurmaResponseDto(int Id, string Nome, Turno Turno, int AnoLetivo, bool Ativo);
