using SistemaDeControle.Application.DTOs.Frequencia;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Application.Interfaces;

public interface IFrequenciaService
{
    Task<List<RegistroFrequenciaResponseDto>> ListAsync(int? professorId, DateOnly? dataInicio, DateOnly? dataFim, StatusFrequencia? status, CancellationToken cancellationToken = default);
    Task<RegistroFrequenciaResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RegistroFrequenciaResponseDto> RegistrarAsync(RegistroFrequenciaCreateDto dto, int usuarioId, CancellationToken cancellationToken = default);
    Task<RegistroFrequenciaResponseDto> AtualizarAsync(int id, RegistroFrequenciaUpdateDto dto, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task<List<OcorrenciaDiaDto>> GetOcorrenciasDoDiaAsync(DateOnly data, CancellationToken cancellationToken = default);
    Task<List<TurmaAfetadaDto>> GetTurmasAfetadasAsync(DateOnly data, int? professorId, CancellationToken cancellationToken = default);
}
