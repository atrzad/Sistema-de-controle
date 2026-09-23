using SistemaDeControle.Application.Common;
using SistemaDeControle.Application.Common.Exceptions;
using SistemaDeControle.Application.DTOs.Justificativas;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Application.Services;

public class JustificativaService : IJustificativaService
{
    // Assinaturas (magic bytes) dos tipos aceitos. O tipo do arquivo é decidido pelo conteúdo,
    // não pelo Content-Type/extensão enviados pelo cliente, que podem ser forjados.
    private static readonly (string TipoConteudo, byte[] Assinatura)[] TiposPermitidos =
    {
        ("application/pdf", "%PDF-"u8.ToArray()),
        ("image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF }),
        ("image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
    };

    private const long TamanhoMaximoBytes = 5 * 1024 * 1024; // 5 MB

    private readonly IJustificativaRepository _justificativaRepository;
    private readonly IFrequenciaRepository _frequenciaRepository;
    private readonly IArquivoStorageService _arquivoStorageService;

    public JustificativaService(
        IJustificativaRepository justificativaRepository,
        IFrequenciaRepository frequenciaRepository,
        IArquivoStorageService arquivoStorageService)
    {
        _justificativaRepository = justificativaRepository;
        _frequenciaRepository = frequenciaRepository;
        _arquivoStorageService = arquivoStorageService;
    }

    public async Task<ResultadoPaginado<JustificativaResponseDto>> ListAsync(int? professorId, DateOnly? dataInicio, DateOnly? dataFim, Paginacao? paginacao = null, CancellationToken cancellationToken = default)
    {
        var (justificativas, total) = await _justificativaRepository.ListAsync(professorId, dataInicio, dataFim, paginacao, cancellationToken);
        return new ResultadoPaginado<JustificativaResponseDto>(justificativas.Select(MapToDto).ToList(), total);
    }

    public async Task<JustificativaResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var justificativa = await _justificativaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Justificativa), id);

        return MapToDto(justificativa);
    }

    public async Task<JustificativaResponseDto> CriarAsync(
        int registroFrequenciaId, string motivo,
        Stream arquivoStream, string nomeArquivoOriginal, string tipoConteudo, long tamanhoBytes,
        CancellationToken cancellationToken = default)
    {
        ValidarTamanho(tamanhoBytes);

        var registro = await _frequenciaRepository.GetByIdAsync(registroFrequenciaId, cancellationToken)
            ?? throw new NotFoundException(nameof(RegistroFrequencia), registroFrequenciaId);

        var existente = await _justificativaRepository.GetByRegistroFrequenciaIdAsync(registroFrequenciaId, cancellationToken);
        if (existente is not null)
            throw new ConflictException("Já existe uma justificativa para este registro de frequência.");

        (arquivoStream, tipoConteudo) = await DetectarTipoAsync(arquivoStream, cancellationToken);
        var arquivoSalvo = await _arquivoStorageService.SalvarAsync(arquivoStream, nomeArquivoOriginal, tipoConteudo, cancellationToken);

        var justificativa = new Justificativa
        {
            RegistroFrequenciaId = registroFrequenciaId,
            RegistroFrequencia = registro,
            Motivo = motivo,
            DataEnvio = DateTime.UtcNow,
        };

        justificativa.Anexos.Add(new Anexo
        {
            NomeArquivoOriginal = nomeArquivoOriginal,
            NomeArquivoArmazenado = arquivoSalvo.NomeArquivoArmazenado,
            CaminhoRelativo = arquivoSalvo.CaminhoRelativo,
            TipoConteudo = tipoConteudo,
            TamanhoBytes = arquivoSalvo.TamanhoBytes,
        });

        await _justificativaRepository.AddAsync(justificativa, cancellationToken);
        await SalvarOuDescartarArquivoAsync(arquivoSalvo, cancellationToken);

        return MapToDto(justificativa);
    }

    public async Task<AnexoResponseDto> AdicionarAnexoAsync(
        int justificativaId, Stream arquivoStream, string nomeArquivoOriginal, string tipoConteudo, long tamanhoBytes,
        CancellationToken cancellationToken = default)
    {
        ValidarTamanho(tamanhoBytes);

        var justificativa = await _justificativaRepository.GetByIdAsync(justificativaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Justificativa), justificativaId);

        (arquivoStream, tipoConteudo) = await DetectarTipoAsync(arquivoStream, cancellationToken);
        var arquivoSalvo = await _arquivoStorageService.SalvarAsync(arquivoStream, nomeArquivoOriginal, tipoConteudo, cancellationToken);

        var anexo = new Anexo
        {
            JustificativaId = justificativaId,
            Justificativa = justificativa,
            NomeArquivoOriginal = nomeArquivoOriginal,
            NomeArquivoArmazenado = arquivoSalvo.NomeArquivoArmazenado,
            CaminhoRelativo = arquivoSalvo.CaminhoRelativo,
            TipoConteudo = tipoConteudo,
            TamanhoBytes = arquivoSalvo.TamanhoBytes,
        };

        justificativa.Anexos.Add(anexo);
        await SalvarOuDescartarArquivoAsync(arquivoSalvo, cancellationToken);

        return new AnexoResponseDto(anexo.Id, anexo.NomeArquivoOriginal, anexo.TipoConteudo, anexo.TamanhoBytes, anexo.CreatedAt);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var justificativa = await _justificativaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Justificativa), id);

        var caminhos = justificativa.Anexos.Select(a => a.CaminhoRelativo).ToList();

        // Banco primeiro: se o commit falhar, os arquivos continuam lá e nada se perde.
        _justificativaRepository.Remove(justificativa);
        await _justificativaRepository.SaveChangesAsync(cancellationToken);

        foreach (var caminho in caminhos)
        {
            try
            {
                _arquivoStorageService.Remover(caminho);
            }
            catch (IOException)
            {
                // Arquivo órfão no disco é inofensivo; o registro já foi removido.
            }
        }
    }

    public async Task<(Stream Conteudo, string TipoConteudo, string NomeArquivoOriginal)> ObterAnexoAsync(int justificativaId, CancellationToken cancellationToken = default)
    {
        var justificativa = await _justificativaRepository.GetByIdAsync(justificativaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Justificativa), justificativaId);

        var anexo = justificativa.Anexos.OrderByDescending(a => a.CreatedAt).FirstOrDefault()
            ?? throw new NotFoundException("Anexo", justificativaId);

        var (conteudo, _, _) = await _arquivoStorageService.ObterAsync(anexo.CaminhoRelativo, cancellationToken);
        return (conteudo, anexo.TipoConteudo, anexo.NomeArquivoOriginal);
    }

    private async Task SalvarOuDescartarArquivoAsync(ArquivoSalvoResultado arquivoSalvo, CancellationToken cancellationToken)
    {
        try
        {
            await _justificativaRepository.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Sem registro no banco o arquivo ficaria órfão no disco.
            _arquivoStorageService.Remover(arquivoSalvo.CaminhoRelativo);
            throw;
        }
    }

    private static async Task<(Stream Conteudo, string TipoConteudo)> DetectarTipoAsync(Stream conteudo, CancellationToken cancellationToken)
    {
        if (!conteudo.CanSeek)
        {
            var copia = new MemoryStream();
            await conteudo.CopyToAsync(copia, cancellationToken);
            copia.Position = 0;
            conteudo = copia;
        }

        var cabecalho = new byte[8];
        var lidos = await conteudo.ReadAtLeastAsync(cabecalho, cabecalho.Length, throwOnEndOfStream: false, cancellationToken);
        conteudo.Position = 0;

        foreach (var (tipo, assinatura) in TiposPermitidos)
        {
            if (lidos >= assinatura.Length && cabecalho.AsSpan(0, assinatura.Length).SequenceEqual(assinatura))
                return (conteudo, tipo);
        }

        throw new BadRequestAppException("Tipo de arquivo não permitido. Envie um PDF, JPG ou PNG.");
    }

    private static void ValidarTamanho(long tamanhoBytes)
    {
        if (tamanhoBytes <= 0)
            throw new BadRequestAppException("Arquivo vazio.");

        if (tamanhoBytes > TamanhoMaximoBytes)
            throw new BadRequestAppException("Arquivo excede o tamanho máximo permitido de 5MB.");
    }

    private static JustificativaResponseDto MapToDto(Justificativa justificativa) => new(
        justificativa.Id,
        justificativa.RegistroFrequenciaId,
        justificativa.RegistroFrequencia.ProfessorId,
        justificativa.RegistroFrequencia.Professor.Nome,
        justificativa.RegistroFrequencia.Data,
        justificativa.Motivo,
        justificativa.DataEnvio,
        justificativa.Anexos.Select(a => new AnexoResponseDto(a.Id, a.NomeArquivoOriginal, a.TipoConteudo, a.TamanhoBytes, a.CreatedAt)).ToList());
}
