using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
}
