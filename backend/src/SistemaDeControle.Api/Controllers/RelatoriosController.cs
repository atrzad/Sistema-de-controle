using Microsoft.AspNetCore.Mvc;
using SistemaDeControle.Application.DTOs.Frequencia;
using SistemaDeControle.Application.DTOs.Relatorios;
using SistemaDeControle.Application.Interfaces;

namespace SistemaDeControle.Api.Controllers;

[ApiController]
[Route("api/v1/relatorios")]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService) => _relatorioService = relatorioService;

    [HttpGet("frequencia/semanal")]
    public async Task<ActionResult<RelatorioFrequenciaDto>> GetSemanal(
        [FromQuery] int professorId, [FromQuery] DateOnly dataReferencia, CancellationToken cancellationToken) =>
        Ok(await _relatorioService.GetRelatorioSemanalAsync(professorId, dataReferencia, cancellationToken));

    [HttpGet("frequencia/mensal")]
    public async Task<ActionResult<RelatorioFrequenciaDto>> GetMensal(
        [FromQuery] int professorId, [FromQuery] int mes, [FromQuery] int ano, CancellationToken cancellationToken) =>
        Ok(await _relatorioService.GetRelatorioMensalAsync(professorId, mes, ano, cancellationToken));

    [HttpGet("frequencia/consolidado")]
    public async Task<ActionResult<List<RelatorioFrequenciaDto>>> GetConsolidado(
        [FromQuery] DateOnly dataInicio, [FromQuery] DateOnly dataFim, CancellationToken cancellationToken) =>
        Ok(await _relatorioService.GetRelatorioConsolidadoAsync(dataInicio, dataFim, cancellationToken));

    [HttpGet("turmas-afetadas")]
    public async Task<ActionResult<List<TurmaAfetadaDto>>> GetTurmasAfetadasHistorico(
        [FromQuery] DateOnly dataInicio, [FromQuery] DateOnly dataFim, CancellationToken cancellationToken) =>
        Ok(await _relatorioService.GetTurmasAfetadasHistoricoAsync(dataInicio, dataFim, cancellationToken));
}
