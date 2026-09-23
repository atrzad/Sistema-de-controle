using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.Common;

public record ContagemFrequencia(int ProfessorId, StatusFrequencia Status, int Total);
