using Microsoft.AspNetCore.Mvc;
using SistemaDeControle.Api.Extensions;
using SistemaDeControle.Application.DTOs.Frequencia;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Api.Controllers;

[ApiController]
[Route("api/v1/frequencia")]
public class FrequenciaController : ControllerBase
{
    private readonly IFrequenciaService _frequenciaService;

    public FrequenciaController(IFrequenciaService frequenciaService) => _frequenciaService = frequenciaService;

    [HttpGet]
    public async Task<ActionResult<List<RegistroFrequenciaResponseDto>>> List(
        [FromQuery] int? professorId, [FromQuery] DateOnly? dataInicio, [FromQuery] DateOnly? dataFim, [FromQuery] StatusFrequencia? status,
        CancellationToken cancellationToken) =>
        Ok(await _frequenciaService.ListAsync(professorId, dataInicio, dataFim, status, cancellationToken));

    [HttpGet("dia/{data}")]
    public async Task<ActionResult<List<OcorrenciaDiaDto>>> GetOcorrenciasDoDia(DateOnly data, CancellationToken cancellationToken) =>
        Ok(await _frequenciaService.GetOcorrenciasDoDiaAsync(data, cancellationToken));

    [HttpGet("turmas-afetadas")]
    public async Task<ActionResult<List<TurmaAfetadaDto>>> GetTurmasAfetadas(
        [FromQuery] DateOnly data, [FromQuery] int? professorId, CancellationToken cancellationToken) =>
        Ok(await _frequenciaService.GetTurmasAfetadasAsync(data, professorId, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RegistroFrequenciaResponseDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await _frequenciaService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<RegistroFrequenciaResponseDto>> Registrar(RegistroFrequenciaCreateDto dto, CancellationToken cancellationToken)
    {
        var criado = await _frequenciaService.RegistrarAsync(dto, User.GetUsuarioId(), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RegistroFrequenciaResponseDto>> Atualizar(int id, RegistroFrequenciaUpdateDto dto, CancellationToken cancellationToken) =>
        Ok(await _frequenciaService.AtualizarAsync(id, dto, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _frequenciaService.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
