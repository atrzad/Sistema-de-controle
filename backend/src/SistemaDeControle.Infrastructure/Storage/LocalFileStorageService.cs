using Microsoft.Extensions.Configuration;
using SistemaDeControle.Application.Interfaces;

namespace SistemaDeControle.Infrastructure.Storage;

public class LocalFileStorageService : IArquivoStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _basePath = configuration["Storage:BasePath"] ?? "storage";
        Directory.CreateDirectory(_basePath);
    }

    public async Task<ArquivoSalvoResultado> SalvarAsync(Stream conteudo, string nomeArquivoOriginal, string tipoConteudo, CancellationToken cancellationToken = default)
    {
        var hoje = DateTime.UtcNow;
        var subPasta = Path.Combine("atestados", hoje.Year.ToString(), hoje.Month.ToString("D2"));
        var pastaCompleta = Path.Combine(_basePath, subPasta);
        Directory.CreateDirectory(pastaCompleta);

        var extensao = Path.GetExtension(nomeArquivoOriginal);
        var nomeArmazenado = $"{Guid.NewGuid():N}{extensao}";
        var caminhoRelativo = Path.Combine(subPasta, nomeArmazenado).Replace('\\', '/');
        var caminhoCompleto = Path.Combine(_basePath, caminhoRelativo);

        await using (var fileStream = new FileStream(caminhoCompleto, FileMode.Create, FileAccess.Write))
        {
            await conteudo.CopyToAsync(fileStream, cancellationToken);
        }

        var tamanho = new FileInfo(caminhoCompleto).Length;

        return new ArquivoSalvoResultado(nomeArmazenado, caminhoRelativo, tamanho);
    }

    public Task<(Stream Conteudo, string TipoConteudo, string NomeArquivoOriginal)> ObterAsync(string caminhoRelativo, CancellationToken cancellationToken = default)
    {
        var caminhoCompleto = ResolverCaminhoSeguro(caminhoRelativo);

        if (!File.Exists(caminhoCompleto))
            throw new FileNotFoundException("Arquivo de anexo não encontrado.", caminhoRelativo);

        Stream stream = new FileStream(caminhoCompleto, FileMode.Open, FileAccess.Read);
        var tipoConteudo = "application/octet-stream";
        var nomeOriginal = Path.GetFileName(caminhoCompleto);

        return Task.FromResult((stream, tipoConteudo, nomeOriginal));
    }

    public void Remover(string caminhoRelativo)
    {
        var caminhoCompleto = ResolverCaminhoSeguro(caminhoRelativo);
        if (File.Exists(caminhoCompleto))
            File.Delete(caminhoCompleto);
    }

    private string ResolverCaminhoSeguro(string caminhoRelativo)
    {
        var raizCompleta = Path.GetFullPath(_basePath);
        var caminhoCompleto = Path.GetFullPath(Path.Combine(_basePath, caminhoRelativo));

        if (!caminhoCompleto.StartsWith(raizCompleta, StringComparison.Ordinal))
            throw new UnauthorizedAccessException("Caminho de arquivo inválido.");

        return caminhoCompleto;
    }
}
