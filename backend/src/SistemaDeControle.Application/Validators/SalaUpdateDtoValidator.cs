using FluentValidation;
using SistemaDeControle.Application.DTOs.Salas;

namespace SistemaDeControle.Application.Validators;

public class SalaUpdateDtoValidator : AbstractValidator<SalaUpdateDto>
{
    public SalaUpdateDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Capacidade).GreaterThan(0).When(x => x.Capacidade.HasValue);
    }
}
