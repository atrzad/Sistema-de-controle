using SistemaDeControle.Domain.Common;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Domain.Entities;

/// <summary>
/// Ocorrência concreta, em uma data específica, de uma <see cref="AulaAgendada"/>:
/// registra se o professor esteve presente ou ausente naquele dia.
/// </summary>
public class RegistroFrequencia : BaseEntity
{
    public int AulaAgendadaId { get; set; }
    public AulaAgendada AulaAgendada { get; set; } = null!;

    public int ProfessorId { get; set; }
    public Professor Professor { get; set; } = null!;

    public DateOnly Data { get; set; }
    public StatusFrequencia Status { get; set; }

    public int RegistradoPorUsuarioId { get; set; }
    public Usuario RegistradoPorUsuario { get; set; } = null!;

    public string? Observacao { get; set; }

    public Justificativa? Justificativa { get; set; }
}
