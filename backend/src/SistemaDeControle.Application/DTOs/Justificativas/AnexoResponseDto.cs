namespace SistemaDeControle.Application.DTOs.Justificativas;

public record AnexoResponseDto(int Id, string NomeArquivoOriginal, string TipoConteudo, long TamanhoBytes, DateTime CreatedAt);
