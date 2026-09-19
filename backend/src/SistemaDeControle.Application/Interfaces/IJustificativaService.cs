using SistemaDeControle.Application.DTOs.Justificativas;

namespace SistemaDeControle.Application.Interfaces;

public interface IJustificativaService
{
    Task<List<JustificativaResponseDto>> ListAsync(int? professorId, DateOnly? dataInicio, DateOnly? dataFim, CancellationToken cancellationToken = default);
    Task<JustificativaResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<JustificativaResponseDto> CriarAsync(
        int registroFrequenciaId, string motivo,
        Stream arquivoStream, string nomeArquivoOriginal, string tipoConteudo, long tamanhoBytes,
        CancellationToken cancellationToken = default);

    Task<AnexoResponseDto> AdicionarAnexoAsync(
        int justificativaId, Stream arquivoStream, string nomeArquivoOriginal, string tipoConteudo, long tamanhoBytes,
        CancellationToken cancellationToken = default);

    Task RemoverAsync(int id, CancellationToken cancellationToken = default);

    Task<(Stream Conteudo, string TipoConteudo, string NomeArquivoOriginal)> ObterAnexoAsync(int justificativaId, CancellationToken cancellationToken = default);
}
