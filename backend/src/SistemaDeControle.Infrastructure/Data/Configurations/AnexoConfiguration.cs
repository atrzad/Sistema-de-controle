using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Infrastructure.Data.Configurations;

public class AnexoConfiguration : IEntityTypeConfiguration<Anexo>
{
    public void Configure(EntityTypeBuilder<Anexo> builder)
    {
        builder.ToTable("anexos");

        builder.Property(a => a.NomeArquivoOriginal).IsRequired().HasMaxLength(255);
        builder.Property(a => a.NomeArquivoArmazenado).IsRequired().HasMaxLength(255);
        builder.Property(a => a.CaminhoRelativo).IsRequired().HasMaxLength(500);
        builder.Property(a => a.TipoConteudo).IsRequired().HasMaxLength(100);

        builder.HasOne(a => a.Justificativa)
            .WithMany(j => j.Anexos)
            .HasForeignKey(a => a.JustificativaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
