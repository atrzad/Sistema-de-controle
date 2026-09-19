using SistemaDeControle.Domain.Common;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Domain.Entities;

public class Turma : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public Turno Turno { get; set; }
    public int AnoLetivo { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<AulaAgendada> AulasAgendadas { get; set; } = new List<AulaAgendada>();
}
