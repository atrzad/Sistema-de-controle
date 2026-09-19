using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Infrastructure.Data.Configurations;

public class SalaConfiguration : IEntityTypeConfiguration<Sala>
{
    public void Configure(EntityTypeBuilder<Sala> builder)
    {
        builder.ToTable("salas");

        builder.Property(s => s.Nome).IsRequired().HasMaxLength(100);

        builder.HasIndex(s => s.Nome).IsUnique();
    }
}
