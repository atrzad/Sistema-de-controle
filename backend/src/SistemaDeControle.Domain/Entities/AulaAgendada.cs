using SistemaDeControle.Domain.Common;
using SistemaDeControle.Domain.Enums;

namespace SistemaDeControle.Domain.Entities;

/// <summary>
/// Slot recorrente da grade horária semanal (o "cronograma"). Não representa uma data
/// concreta — para a ocorrência real em um dia específico, ver <see cref="RegistroFrequencia"/>.
/// </summary>
public class AulaAgendada : BaseEntity
{
    public int ProfessorId { get; set; }
    public Professor Professor { get; set; } = null!;

    public int TurmaId { get; set; }
    public Turma Turma { get; set; } = null!;

    public int SalaId { get; set; }
    public Sala Sala { get; set; } = null!;

    public DiaSemana DiaSemana { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }

    public string? Disciplina { get; set; }
    public DateOnly? VigenteDesde { get; set; }
    public DateOnly? VigenteAte { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<RegistroFrequencia> RegistrosFrequencia { get; set; } = new List<RegistroFrequencia>();
}
