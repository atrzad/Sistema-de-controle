namespace SistemaDeControle.Application.DTOs.Auth;

public record LoginResponseDto(string Token, DateTime ExpiresAtUtc, UsuarioDto Usuario);
