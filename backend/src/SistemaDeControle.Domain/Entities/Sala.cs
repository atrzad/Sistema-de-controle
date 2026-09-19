using SistemaDeControle.Domain.Common;

namespace SistemaDeControle.Domain.Entities;

public class Sala : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public int? Capacidade { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<AulaAgendada> AulasAgendadas { get; set; } = new List<AulaAgendada>();
}
