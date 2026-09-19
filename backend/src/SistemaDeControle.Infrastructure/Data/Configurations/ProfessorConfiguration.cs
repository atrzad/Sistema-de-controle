using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Infrastructure.Data.Configurations;

public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        builder.ToTable("professores");

        builder.Property(p => p.Nome).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Telefone).HasMaxLength(20);
        builder.Property(p => p.Matricula).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Disciplina).HasMaxLength(100);

        builder.HasIndex(p => p.Email).IsUnique();
        builder.HasIndex(p => p.Matricula).IsUnique();
    }
}
