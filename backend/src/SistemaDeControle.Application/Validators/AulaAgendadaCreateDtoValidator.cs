using FluentValidation;
using SistemaDeControle.Application.DTOs.Cronograma;

namespace SistemaDeControle.Application.Validators;

public class AulaAgendadaCreateDtoValidator : AbstractValidator<AulaAgendadaCreateDto>
{
    public AulaAgendadaCreateDtoValidator()
    {
        RuleFor(x => x.ProfessorId).GreaterThan(0);
        RuleFor(x => x.TurmaId).GreaterThan(0);
        RuleFor(x => x.SalaId).GreaterThan(0);
        RuleFor(x => x.DiaSemana).IsInEnum();
        RuleFor(x => x.Disciplina).MaximumLength(100);
        RuleFor(x => x.HoraFim)
            .GreaterThan(x => x.HoraInicio)
            .WithMessage("O horário final deve ser maior que o horário inicial.");
        RuleFor(x => x.VigenteAte)
            .GreaterThanOrEqualTo(x => x.VigenteDesde!.Value)
            .When(x => x.VigenteDesde.HasValue && x.VigenteAte.HasValue)
            .WithMessage("A data de vigência final deve ser maior ou igual à inicial.");
    }
}
