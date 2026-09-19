using Microsoft.AspNetCore.Mvc;
using SistemaDeControle.Application.DTOs.Cronograma;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Api.Controllers;

[ApiController]
[Route("api/v1/cronograma")]
public class CronogramaController : ControllerBase
{
    private readonly ICronogramaService _cronogramaService;

    public CronogramaController(ICronogramaService cronogramaService) => _cronogramaService = cronogramaService;

    [HttpGet]
    public async Task<ActionResult<List<AulaAgendadaResponseDto>>> List(
        [FromQuery] int? professorId, [FromQuery] int? turmaId, [FromQuery] int? salaId, [FromQuery] DiaSemana? diaSemana,
        CancellationToken cancellationToken) =>
        Ok(await _cronogramaService.ListAsync(professorId, turmaId, salaId, diaSemana, cancellationToken));

    [HttpGet("grade-semanal")]
    public async Task<ActionResult<List<GradeDiaDto>>> GetGradeSemanal(CancellationToken cancellationToken) =>
        Ok(await _cronogramaService.GetGradeSemanalAsync(cancellationToken));

    [HttpGet("professor/{professorId:int}")]
    public async Task<ActionResult<List<AulaAgendadaResponseDto>>> GetPorProfessor(int professorId, CancellationToken cancellationToken) =>
        Ok(await _cronogramaService.GetCronogramaDoProfessorAsync(professorId, cancellationToken));

    [HttpGet("turma/{turmaId:int}")]
    public async Task<ActionResult<List<AulaAgendadaResponseDto>>> GetPorTurma(int turmaId, CancellationToken cancellationToken) =>
        Ok(await _cronogramaService.GetCronogramaDaTurmaAsync(turmaId, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AulaAgendadaResponseDto>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await _cronogramaService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<AulaAgendadaResponseDto>> Create(AulaAgendadaCreateDto dto, CancellationToken cancellationToken)
    {
        var criada = await _cronogramaService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = criada.Id }, criada);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AulaAgendadaResponseDto>> Update(int id, AulaAgendadaUpdateDto dto, CancellationToken cancellationToken) =>
        Ok(await _cronogramaService.UpdateAsync(id, dto, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Inativar(int id, CancellationToken cancellationToken)
    {
        await _cronogramaService.InativarAsync(id, cancellationToken);
        return NoContent();
    }
}
