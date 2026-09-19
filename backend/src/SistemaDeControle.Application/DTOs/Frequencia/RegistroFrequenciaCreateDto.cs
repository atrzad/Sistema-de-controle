using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.DTOs.Frequencia;

public record RegistroFrequenciaCreateDto(int AulaAgendadaId, DateOnly Data, StatusFrequencia Status, string? Observacao);
