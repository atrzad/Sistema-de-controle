namespace SistemaDeControle.Application.Common;

/// <summary>Página solicitada numa listagem (1-based). Tamanho limitado a <see cref="TamanhoMaximo"/>.</summary>
public record Paginacao
{
    public const int TamanhoMaximo = 500;

    public int Pagina { get; }
    public int TamanhoPagina { get; }

    public Paginacao(int pagina, int tamanhoPagina)
    {
        Pagina = Math.Max(1, pagina);
        TamanhoPagina = Math.Clamp(tamanhoPagina, 1, TamanhoMaximo);
    }

    public int Skip => (Pagina - 1) * TamanhoPagina;

    /// <summary>Null quando nenhum parâmetro foi informado (listagem completa, comportamento original).</summary>
    public static Paginacao? De(int? pagina, int? tamanhoPagina) =>
        pagina is null && tamanhoPagina is null ? null : new Paginacao(pagina ?? 1, tamanhoPagina ?? 50);
}

public record ResultadoPaginado<T>(List<T> Itens, int Total);
