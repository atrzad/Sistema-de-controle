using FluentValidation;
using SistemaDeControle.Application.DTOs.Professores;

namespace SistemaDeControle.Application.Validators;

public class ProfessorUpdateDtoValidator : AbstractValidator<ProfessorUpdateDto>
{
    public ProfessorUpdateDtoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Telefone).MaximumLength(20);
        RuleFor(x => x.Matricula).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Disciplina).MaximumLength(100);
    }
}
