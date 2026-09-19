using SistemaDeControle.Domain.Common;

namespace SistemaDeControle.Domain.Entities;

public class Professor : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Matricula { get; set; } = string.Empty;
    public string? Disciplina { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<AulaAgendada> AulasAgendadas { get; set; } = new List<AulaAgendada>();
    public ICollection<RegistroFrequencia> RegistrosFrequencia { get; set; } = new List<RegistroFrequencia>();
}
