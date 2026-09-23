using FluentValidation;
using SistemaDeControle.Application.DTOs.Auth;

namespace SistemaDeControle.Application.Validators.Auth;

public class AlterarSenhaRequestDtoValidator : AbstractValidator<AlterarSenhaRequestDto>
{
    public AlterarSenhaRequestDtoValidator()
    {
        RuleFor(x => x.SenhaAtual).NotEmpty();

        RuleFor(x => x.NovaSenha)
            .NotEmpty()
            .MinimumLength(10).WithMessage("A nova senha deve ter no mínimo 10 caracteres.")
            .MaximumLength(128)
            .Matches("[A-Za-z]").WithMessage("A nova senha deve conter ao menos uma letra.")
            .Matches("[0-9]").WithMessage("A nova senha deve conter ao menos um número.")
            .NotEqual(x => x.SenhaAtual).WithMessage("A nova senha deve ser diferente da atual.");
    }
}
