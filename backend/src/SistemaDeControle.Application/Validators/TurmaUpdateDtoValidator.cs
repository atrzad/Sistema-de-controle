using FluentValidation;
using SistemaDeControle.Application.DTOs.Turmas;

namespace SistemaDeControle.Application.Validators;

public class TurmaUpdateDtoValidator : AbstractValidator<TurmaUpdateDto>
{
    public TurmaUpdateDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Turno).IsInEnum();
        RuleFor(x => x.AnoLetivo).InclusiveBetween(2000, 2100);
    }
}
