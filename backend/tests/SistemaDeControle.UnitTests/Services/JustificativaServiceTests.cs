using Moq;
using SistemaDeControle.Application.Common.Exceptions;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Application.Services;
using SistemaDeControle.Domain.Entities;
using SistemaDeControle.Domain.Enums;
using Xunit;

namespace SistemaDeControle.UnitTests.Services;

public class JustificativaServiceTests
{
    private static readonly byte[] PdfValido = "%PDF-1.7\nconteudo"u8.ToArray();
    private static readonly byte[] PngValido = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x01 };

    private readonly Mock<IJustificativaRepository> _justificativaRepository = new();
    private readonly Mock<IFrequenciaRepository> _frequenciaRepository = new();
    private readonly Mock<IArquivoStorageService> _storage = new();

    private JustificativaService CriarService() =>
        new(_justificativaRepository.Object, _frequenciaRepository.Object, _storage.Object);

    private static RegistroFrequencia NovoRegistro(int id = 10)
    {
        var professor = new Professor { Id = 1, Nome = "Prof. Teste", Email = "p@t.com", Matricula = "M1" };
        return new RegistroFrequencia
        {
            Id = id, ProfessorId = 1, Professor = professor,
            Data = new DateOnly(2026, 9, 21), Status = StatusFrequencia.Ausente,
        };
    }

    private void ConfigurarRegistroExistente(RegistroFrequencia registro)
    {
        _frequenciaRepository
            .Setup(r => r.GetByIdAsync(registro.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(registro);

        _storage
            .Setup(s => s.SalvarAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ArquivoSalvoResultado("abc.pdf", "atestados/2026/09/abc.pdf", 10));
    }

    [Fact]
    public async Task CriarAsync_DeveUsarTipoDetectadoPeloConteudo_E_IgnorarContentTypeDoCliente()
    {
        var registro = NovoRegistro();
        ConfigurarRegistroExistente(registro);

        // Cliente diz que é PDF, mas o conteúdo é PNG.
        var resultado = await CriarService().CriarAsync(
            registro.Id, "Consulta médica", new MemoryStream(PngValido), "atestado.pdf", "application/pdf", PngValido.Length);

        Assert.Equal("image/png", resultado.Anexos.Single().TipoConteudo);
        _storage.Verify(s => s.SalvarAsync(It.IsAny<Stream>(), "atestado.pdf", "image/png", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveRejeitarArquivoComAssinaturaDesconhecida()
    {
        var registro = NovoRegistro();
        ConfigurarRegistroExistente(registro);
        var executavel = "MZ\x90\0\x03\0\0\0"u8.ToArray();

        await Assert.ThrowsAsync<BadRequestAppException>(() => CriarService().CriarAsync(
            registro.Id, "Motivo", new MemoryStream(executavel), "atestado.pdf", "application/pdf", executavel.Length));

        _storage.Verify(s => s.SalvarAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveRejeitarArquivoAcimaDe5MB()
    {
        await Assert.ThrowsAsync<BadRequestAppException>(() => CriarService().CriarAsync(
            1, "Motivo", new MemoryStream(PdfValido), "a.pdf", "application/pdf", 6 * 1024 * 1024));
    }

    [Fact]
    public async Task CriarAsync_QuandoBancoFalha_DeveRemoverArquivoJaGravado()
    {
        var registro = NovoRegistro();
        ConfigurarRegistroExistente(registro);
        _justificativaRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("falha no banco"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => CriarService().CriarAsync(
            registro.Id, "Motivo", new MemoryStream(PdfValido), "a.pdf", "application/pdf", PdfValido.Length));

        _storage.Verify(s => s.Remover("atestados/2026/09/abc.pdf"), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveApagarArquivosSomenteDepoisDoCommit()
    {
        var justificativa = new Justificativa
        {
            Id = 5, RegistroFrequencia = NovoRegistro(),
            Anexos = { new Anexo { CaminhoRelativo = "atestados/x.pdf" } },
        };
        _justificativaRepository
            .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(justificativa);
        _justificativaRepository
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("falha no banco"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => CriarService().RemoverAsync(5));

        _storage.Verify(s => s.Remover(It.IsAny<string>()), Times.Never);
    }
}
