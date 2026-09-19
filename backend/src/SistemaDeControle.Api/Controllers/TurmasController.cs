using Microsoft.AspNetCore.Mvc;
using SistemaDeControle.Application.DTOs.Turmas;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Api.Controllers;

[ApiController]
[Route("api/v1/turmas")]
public class TurmasController : ControllerBase
{
    private readonly ITurmaService _turmaService;

    public TurmasController(ITurmaService turmaService) => _turmaService = turmaService;

    [HttpGet]
    public async Task<ActionResult<List<TurmaResponseDto>>> List(
        [FromQuery] int? anoLetivo, [FromQuery] Turno? turno, [FromQuery] bool? ativo, CancellationToken cancellationToken) =>
        Ok(await _turmaService.ListAsync(anoLetivo, turno, ativo, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TurmaResponseDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await _turmaService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<TurmaResponseDto>> Create(TurmaCreateDto dto, CancellationToken cancellationToken)
    {
        var criada = await _turmaService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = criada.Id }, criada);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TurmaResponseDto>> Update(int id, TurmaUpdateDto dto, CancellationToken cancellationToken) =>
        Ok(await _turmaService.UpdateAsync(id, dto, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Inativar(int id, CancellationToken cancellationToken)
    {
        await _turmaService.InativarAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:int}/professores")]
    public async Task<IActionResult> GetProfessores(int id, CancellationToken cancellationToken) =>
        Ok(await _turmaService.GetProfessoresDaTurmaAsync(id, cancellationToken));
}
