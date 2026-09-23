using SistemaDeControle.Application.DTOs.Auth;
using SistemaDeControle.Application.Validators.Auth;
using Xunit;

namespace SistemaDeControle.UnitTests.Validators;

public class AlterarSenhaRequestDtoValidatorTests
{
    private readonly AlterarSenhaRequestDtoValidator _validator = new();

    [Theory]
    [InlineData("curta1")]           // menos de 10 caracteres
    [InlineData("somenteletras")]    // sem número
    [InlineData("1234567890")]       // sem letra
    [InlineData("SenhaAtual123")]    // igual à atual
    public void DeveRejeitarSenhaFraca(string novaSenha)
    {
        var resultado = _validator.Validate(new AlterarSenhaRequestDto("SenhaAtual123", novaSenha));
        Assert.False(resultado.IsValid);
    }

    [Fact]
    public void DeveAceitarSenhaForte()
    {
        var resultado = _validator.Validate(new AlterarSenhaRequestDto("SenhaAtual123", "NovaSenhaForte2026"));
        Assert.True(resultado.IsValid);
    }
}
