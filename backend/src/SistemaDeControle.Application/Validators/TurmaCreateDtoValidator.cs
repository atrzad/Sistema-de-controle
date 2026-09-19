using FluentValidation;
using SistemaDeControle.Application.DTOs.Turmas;

namespace SistemaDeControle.Application.Validators;

public class TurmaCreateDtoValidator : AbstractValidator<TurmaCreateDto>
{
    public TurmaCreateDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Turno).IsInEnum();
        RuleFor(x => x.AnoLetivo).InclusiveBetween(2000, 2100);
    }
}
