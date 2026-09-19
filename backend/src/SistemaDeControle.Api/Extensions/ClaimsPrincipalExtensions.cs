using System.Security.Claims;

namespace SistemaDeControle.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUsuarioId(this ClaimsPrincipal user)
    {
        var valor = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        return int.Parse(valor ?? throw new InvalidOperationException("Usuário autenticado sem claim de id."));
    }
}
