using SistemaDeControle.Domain.Common;

namespace SistemaDeControle.Domain.Entities;

public class Anexo : BaseEntity
{
    public int JustificativaId { get; set; }
    public Justificativa Justificativa { get; set; } = null!;

    public string NomeArquivoOriginal { get; set; } = string.Empty;
    public string NomeArquivoArmazenado { get; set; } = string.Empty;
    public string CaminhoRelativo { get; set; } = string.Empty;
    public string TipoConteudo { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
}
