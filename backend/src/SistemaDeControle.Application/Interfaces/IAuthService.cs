using SistemaDeControle.Application.DTOs.Auth;

namespace SistemaDeControle.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<UsuarioDto> GetUsuarioAtualAsync(int usuarioId, CancellationToken cancellationToken = default);
    Task AlterarSenhaAsync(int usuarioId, AlterarSenhaRequestDto request, CancellationToken cancellationToken = default);
}
