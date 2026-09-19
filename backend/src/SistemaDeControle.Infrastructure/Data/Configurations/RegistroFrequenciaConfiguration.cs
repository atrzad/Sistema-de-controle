using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Infrastructure.Data.Configurations;

public class RegistroFrequenciaConfiguration : IEntityTypeConfiguration<RegistroFrequencia>
{
    public void Configure(EntityTypeBuilder<RegistroFrequencia> builder)
    {
        builder.ToTable("registros_frequencia");

        builder.Property(r => r.Status).IsRequired().HasConversion<string>().HasMaxLength(30);
        builder.Property(r => r.Observacao).HasMaxLength(500);

        builder.HasOne(r => r.AulaAgendada)
            .WithMany(a => a.RegistrosFrequencia)
            .HasForeignKey(r => r.AulaAgendadaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Professor)
            .WithMany(p => p.RegistrosFrequencia)
            .HasForeignKey(r => r.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RegistradoPorUsuario)
            .WithMany(u => u.RegistrosRegistrados)
            .HasForeignKey(r => r.RegistradoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.AulaAgendadaId, r.Data }).IsUnique();
        builder.HasIndex(r => r.Data);
    }
}
