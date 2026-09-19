using FluentValidation;
using SistemaDeControle.Application.DTOs.Frequencia;

namespace SistemaDeControle.Application.Validators;

public class RegistroFrequenciaCreateDtoValidator : AbstractValidator<RegistroFrequenciaCreateDto>
{
    public RegistroFrequenciaCreateDtoValidator()
    {
        RuleFor(x => x.AulaAgendadaId).GreaterThan(0);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Observacao).MaximumLength(500);
    }
}
