namespace SistemaDeControle.Application.DTOs.Salas;

public record SalaUpdateDto(string Nome, int? Capacidade, bool Ativo);
