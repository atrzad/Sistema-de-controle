namespace SistemaDeControle.Application.DTOs.Salas;

public record SalaResponseDto(int Id, string Nome, int? Capacidade, bool Ativo);
