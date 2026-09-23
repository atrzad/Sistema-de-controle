using Microsoft.AspNetCore.Identity;
using SistemaDeControle.Application.Common.Exceptions;
using SistemaDeControle.Application.DTOs.Auth;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public AuthService(IUsuarioRepository usuarioRepository, IJwtTokenGenerator tokenGenerator)
    {
        _usuarioRepository = usuarioRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (usuario is null)
            throw new UnauthorizedAppException("E-mail ou senha inválidos.");

        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, request.Senha);
        if (resultado == PasswordVerificationResult.Failed)
            throw new UnauthorizedAppException("E-mail ou senha inválidos.");

        var (token, expiresAtUtc) = _tokenGenerator.GenerateToken(usuario);

        return new LoginResponseDto(token, expiresAtUtc, MapToDto(usuario));
    }

    public async Task<UsuarioDto> GetUsuarioAtualAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Usuario), usuarioId);

        return MapToDto(usuario);
    }

    public async Task AlterarSenhaAsync(int usuarioId, AlterarSenhaRequestDto request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(Usuario), usuarioId);

        // 400 (e não 401) para não derrubar a sessão no frontend por um erro de digitação.
        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, request.SenhaAtual);
        if (resultado == PasswordVerificationResult.Failed)
            throw new BadRequestAppException("Senha atual incorreta.");

        usuario.SenhaHash = _passwordHasher.HashPassword(usuario, request.NovaSenha);
        await _usuarioRepository.SaveChangesAsync(cancellationToken);
    }

    private static UsuarioDto MapToDto(Usuario usuario) => new(usuario.Id, usuario.Nome, usuario.Email);
}
