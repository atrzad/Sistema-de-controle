using SistemaDeControle.Domain.Common;

namespace SistemaDeControle.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;

    public ICollection<RegistroFrequencia> RegistrosRegistrados { get; set; } = new List<RegistroFrequencia>();
}
