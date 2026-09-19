using Microsoft.AspNetCore.Mvc;
using SistemaDeControle.Application.DTOs.Salas;
using SistemaDeControle.Application.Interfaces;

namespace SistemaDeControle.Api.Controllers;

[ApiController]
[Route("api/v1/salas")]
public class SalasController : ControllerBase
{
    private readonly ISalaService _salaService;

    public SalasController(ISalaService salaService) => _salaService = salaService;

    [HttpGet]
    public async Task<ActionResult<List<SalaResponseDto>>> List([FromQuery] bool? ativo, CancellationToken cancellationToken) =>
        Ok(await _salaService.ListAsync(ativo, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SalaResponseDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await _salaService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<SalaResponseDto>> Create(SalaCreateDto dto, CancellationToken cancellationToken)
    {
        var criada = await _salaService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = criada.Id }, criada);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SalaResponseDto>> Update(int id, SalaUpdateDto dto, CancellationToken cancellationToken) =>
        Ok(await _salaService.UpdateAsync(id, dto, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Inativar(int id, CancellationToken cancellationToken)
    {
        await _salaService.InativarAsync(id, cancellationToken);
        return NoContent();
    }
}
