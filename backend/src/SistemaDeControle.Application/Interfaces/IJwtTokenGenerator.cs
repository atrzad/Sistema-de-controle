using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(Usuario usuario);
}
