using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.DTOs.Frequencia;

public record RegistroFrequenciaUpdateDto(StatusFrequencia Status, string? Observacao);
