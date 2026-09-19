using FluentValidation;
using SistemaDeControle.Application.DTOs.Frequencia;

namespace SistemaDeControle.Application.Validators;

public class RegistroFrequenciaUpdateDtoValidator : AbstractValidator<RegistroFrequenciaUpdateDto>
{
    public RegistroFrequenciaUpdateDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Observacao).MaximumLength(500);
    }
}
