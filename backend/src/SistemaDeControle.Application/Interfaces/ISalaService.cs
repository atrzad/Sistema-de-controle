using SistemaDeControle.Application.DTOs.Salas;

namespace SistemaDeControle.Application.Interfaces;

public interface ISalaService
{
    Task<List<SalaResponseDto>> ListAsync(bool? ativo, CancellationToken cancellationToken = default);
    Task<SalaResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SalaResponseDto> CreateAsync(SalaCreateDto dto, CancellationToken cancellationToken = default);
    Task<SalaResponseDto> UpdateAsync(int id, SalaUpdateDto dto, CancellationToken cancellationToken = default);
    Task InativarAsync(int id, CancellationToken cancellationToken = default);
}
