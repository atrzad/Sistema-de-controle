using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Infrastructure.Data.Configurations;

public class JustificativaConfiguration : IEntityTypeConfiguration<Justificativa>
{
    public void Configure(EntityTypeBuilder<Justificativa> builder)
    {
        builder.ToTable("justificativas");

        builder.Property(j => j.Motivo).IsRequired().HasMaxLength(1000);

        builder.HasOne(j => j.RegistroFrequencia)
            .WithOne(r => r.Justificativa)
            .HasForeignKey<Justificativa>(j => j.RegistroFrequenciaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(j => j.RegistroFrequenciaId).IsUnique();
    }
}
