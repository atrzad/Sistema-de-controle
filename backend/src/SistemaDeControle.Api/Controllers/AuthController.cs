using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SistemaDeControle.Api.Extensions;
using SistemaDeControle.Application.DTOs.Auth;
using SistemaDeControle.Application.Interfaces;

namespace SistemaDeControle.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingExtensions.LoginPolicy)]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var resultado = await _authService.LoginAsync(request, cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UsuarioDto>> Me(CancellationToken cancellationToken)
    {
        var usuario = await _authService.GetUsuarioAtualAsync(User.GetUsuarioId(), cancellationToken);
        return Ok(usuario);
    }

    [HttpPost("alterar-senha")]
    [EnableRateLimiting(RateLimitingExtensions.LoginPolicy)]
    public async Task<IActionResult> AlterarSenha(AlterarSenhaRequestDto request, CancellationToken cancellationToken)
    {
        await _authService.AlterarSenhaAsync(User.GetUsuarioId(), request, cancellationToken);
        return NoContent();
    }
}
