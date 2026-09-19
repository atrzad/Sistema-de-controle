using SistemaDeControle.Domain.Common;

namespace SistemaDeControle.Domain.Entities;

public class Justificativa : BaseEntity
{
    public int RegistroFrequenciaId { get; set; }
    public RegistroFrequencia RegistroFrequencia { get; set; } = null!;

    public string Motivo { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

    public ICollection<Anexo> Anexos { get; set; } = new List<Anexo>();
}
