using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Infrastructure.Data.Configurations;

public class TurmaConfiguration : IEntityTypeConfiguration<Turma>
{
    public void Configure(EntityTypeBuilder<Turma> builder)
    {
        builder.ToTable("turmas");

        builder.Property(t => t.Nome).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Turno).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(t => new { t.Nome, t.AnoLetivo, t.Turno }).IsUnique();
    }
}
