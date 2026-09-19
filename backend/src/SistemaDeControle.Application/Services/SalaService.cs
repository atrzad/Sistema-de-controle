using SistemaDeControle.Application.Common.Exceptions;
using SistemaDeControle.Application.DTOs.Salas;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Application.Services;

public class SalaService : ISalaService
{
    private readonly ISalaRepository _salaRepository;

    public SalaService(ISalaRepository salaRepository) => _salaRepository = salaRepository;

    public async Task<List<SalaResponseDto>> ListAsync(bool? ativo, CancellationToken cancellationToken = default)
    {
        var salas = await _salaRepository.ListAsync(ativo, cancellationToken);
        return salas.Select(MapToDto).ToList();
    }

    public async Task<SalaResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sala = await _salaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Sala), id);

        return MapToDto(sala);
    }

    public async Task<SalaResponseDto> CreateAsync(SalaCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (await _salaRepository.ExistsByNomeAsync(dto.Nome, cancellationToken: cancellationToken))
            throw new ConflictException($"Já existe uma sala com o nome '{dto.Nome}'.");

        var sala = new Sala { Nome = dto.Nome, Capacidade = dto.Capacidade, Ativo = true };

        await _salaRepository.AddAsync(sala, cancellationToken);
        await _salaRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(sala);
    }

    public async Task<SalaResponseDto> UpdateAsync(int id, SalaUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var sala = await _salaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Sala), id);

        if (await _salaRepository.ExistsByNomeAsync(dto.Nome, id, cancellationToken))
            throw new ConflictException($"Já existe uma sala com o nome '{dto.Nome}'.");

        sala.Nome = dto.Nome;
        sala.Capacidade = dto.Capacidade;
        sala.Ativo = dto.Ativo;

        await _salaRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(sala);
    }

    public async Task InativarAsync(int id, CancellationToken cancellationToken = default)
    {
        var sala = await _salaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Sala), id);

        sala.Ativo = false;
        await _salaRepository.SaveChangesAsync(cancellationToken);
    }

    private static SalaResponseDto MapToDto(Sala sala) => new(sala.Id, sala.Nome, sala.Capacidade, sala.Ativo);
}
