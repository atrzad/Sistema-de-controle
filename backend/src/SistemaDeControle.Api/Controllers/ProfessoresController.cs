using Microsoft.AspNetCore.Mvc;
using SistemaDeControle.Application.DTOs.Professores;
using SistemaDeControle.Application.Interfaces;

namespace SistemaDeControle.Api.Controllers;

[ApiController]
[Route("api/v1/professores")]
public class ProfessoresController : ControllerBase
{
    private readonly IProfessorService _professorService;

    public ProfessoresController(IProfessorService professorService) => _professorService = professorService;

    [HttpGet]
    public async Task<ActionResult<List<ProfessorResponseDto>>> List([FromQuery] string? nome, [FromQuery] bool? ativo, CancellationToken cancellationToken) =>
        Ok(await _professorService.ListAsync(nome, ativo, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProfessorResponseDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await _professorService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<ProfessorResponseDto>> Create(ProfessorCreateDto dto, CancellationToken cancellationToken)
    {
        var criado = await _professorService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProfessorResponseDto>> Update(int id, ProfessorUpdateDto dto, CancellationToken cancellationToken) =>
        Ok(await _professorService.UpdateAsync(id, dto, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Inativar(int id, CancellationToken cancellationToken)
    {
        await _professorService.InativarAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:int}/turmas")]
    public async Task<IActionResult> GetTurmas(int id, CancellationToken cancellationToken) =>
        Ok(await _professorService.GetTurmasDoProfessorAsync(id, cancellationToken));
}
