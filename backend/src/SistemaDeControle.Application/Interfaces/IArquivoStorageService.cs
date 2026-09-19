namespace SistemaDeControle.Application.Interfaces;

public record ArquivoSalvoResultado(string NomeArquivoArmazenado, string CaminhoRelativo, long TamanhoBytes);

public interface IArquivoStorageService
{
    Task<ArquivoSalvoResultado> SalvarAsync(Stream conteudo, string nomeArquivoOriginal, string tipoConteudo, CancellationToken cancellationToken = default);

    Task<(Stream Conteudo, string TipoConteudo, string NomeArquivoOriginal)> ObterAsync(string caminhoRelativo, CancellationToken cancellationToken = default);

    void Remover(string caminhoRelativo);
}
