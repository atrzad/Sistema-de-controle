using SistemaDeControle.Application.Common.Exceptions;
using SistemaDeControle.Application.DTOs.Justificativas;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Application.Services;

public class JustificativaService : IJustificativaService
{
    private static readonly HashSet<string> TiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf", "image/jpeg", "image/png"
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

    public async Task<List<JustificativaResponseDto>> ListAsync(int? professorId, DateOnly? dataInicio, DateOnly? dataFim, CancellationToken cancellationToken = default)
    {
        var justificativas = await _justificativaRepository.ListAsync(professorId, dataInicio, dataFim, cancellationToken);
        return justificativas.Select(MapToDto).ToList();
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
        ValidarArquivo(tipoConteudo, tamanhoBytes);

        var registro = await _frequenciaRepository.GetByIdAsync(registroFrequenciaId, cancellationToken)
            ?? throw new NotFoundException(nameof(RegistroFrequencia), registroFrequenciaId);

        var existente = await _justificativaRepository.GetByRegistroFrequenciaIdAsync(registroFrequenciaId, cancellationToken);
        if (existente is not null)
            throw new ConflictException("Já existe uma justificativa para este registro de frequência.");

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
        await _justificativaRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(justificativa);
    }

    public async Task<AnexoResponseDto> AdicionarAnexoAsync(
        int justificativaId, Stream arquivoStream, string nomeArquivoOriginal, string tipoConteudo, long tamanhoBytes,
        CancellationToken cancellationToken = default)
    {
        ValidarArquivo(tipoConteudo, tamanhoBytes);

        var justificativa = await _justificativaRepository.GetByIdAsync(justificativaId, cancellationToken)
            ?? throw new NotFoundException(nameof(Justificativa), justificativaId);

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
        await _justificativaRepository.SaveChangesAsync(cancellationToken);

        return new AnexoResponseDto(anexo.Id, anexo.NomeArquivoOriginal, anexo.TipoConteudo, anexo.TamanhoBytes, anexo.CreatedAt);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var justificativa = await _justificativaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Justificativa), id);

        foreach (var anexo in justificativa.Anexos)
            _arquivoStorageService.Remover(anexo.CaminhoRelativo);

        _justificativaRepository.Remove(justificativa);
        await _justificativaRepository.SaveChangesAsync(cancellationToken);
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

    private static void ValidarArquivo(string tipoConteudo, long tamanhoBytes)
    {
        if (!TiposPermitidos.Contains(tipoConteudo))
            throw new BadRequestAppException("Tipo de arquivo não permitido. Envie um PDF, JPG ou PNG.");

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
