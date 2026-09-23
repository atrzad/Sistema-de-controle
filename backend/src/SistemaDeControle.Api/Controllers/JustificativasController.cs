using Microsoft.AspNetCore.Mvc;
using SistemaDeControle.Application.Common;
using SistemaDeControle.Application.DTOs.Justificativas;
using SistemaDeControle.Application.Interfaces;

namespace SistemaDeControle.Api.Controllers;

public class JustificativaUploadRequest
{
    public int RegistroFrequenciaId { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public IFormFile Arquivo { get; set; } = null!;
}

public class AnexoUploadRequest
{
    public IFormFile Arquivo { get; set; } = null!;
}

[ApiController]
[Route("api/v1/justificativas")]
public class JustificativasController : ControllerBase
{
    private readonly IJustificativaService _justificativaService;

    public JustificativasController(IJustificativaService justificativaService) => _justificativaService = justificativaService;

    [HttpGet]
    public async Task<ActionResult<List<JustificativaResponseDto>>> List(
        [FromQuery] int? professorId, [FromQuery] DateOnly? dataInicio, [FromQuery] DateOnly? dataFim,
        [FromQuery] int? page, [FromQuery] int? pageSize, CancellationToken cancellationToken)
    {
        var resultado = await _justificativaService.ListAsync(professorId, dataInicio, dataFim, Paginacao.De(page, pageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = resultado.Total.ToString();
        return Ok(resultado.Itens);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JustificativaResponseDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await _justificativaService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<JustificativaResponseDto>> Create([FromForm] JustificativaUploadRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Arquivo.OpenReadStream();
        var criada = await _justificativaService.CriarAsync(
            request.RegistroFrequenciaId, request.Motivo,
            stream, request.Arquivo.FileName, request.Arquivo.ContentType, request.Arquivo.Length,
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = criada.Id }, criada);
    }

    [HttpPost("{id:int}/anexo")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<AnexoResponseDto>> AdicionarAnexo(int id, [FromForm] AnexoUploadRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.Arquivo.OpenReadStream();
        var anexo = await _justificativaService.AdicionarAnexoAsync(
            id, stream, request.Arquivo.FileName, request.Arquivo.ContentType, request.Arquivo.Length, cancellationToken);

        return Ok(anexo);
    }

    [HttpGet("{id:int}/anexo")]
    public async Task<IActionResult> GetAnexo(int id, CancellationToken cancellationToken)
    {
        var (conteudo, tipoConteudo, nomeArquivoOriginal) = await _justificativaService.ObterAnexoAsync(id, cancellationToken);
        return File(conteudo, tipoConteudo, nomeArquivoOriginal);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _justificativaService.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
