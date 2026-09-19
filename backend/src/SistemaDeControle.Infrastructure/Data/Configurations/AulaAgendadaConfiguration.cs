using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Infrastructure.Data.Configurations;

public class AulaAgendadaConfiguration : IEntityTypeConfiguration<AulaAgendada>
{
    public void Configure(EntityTypeBuilder<AulaAgendada> builder)
    {
        builder.ToTable("aulas_agendadas");

        builder.Property(a => a.DiaSemana).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Disciplina).HasMaxLength(100);

        builder.HasOne(a => a.Professor)
            .WithMany(p => p.AulasAgendadas)
            .HasForeignKey(a => a.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Turma)
            .WithMany(t => t.AulasAgendadas)
            .HasForeignKey(a => a.TurmaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Sala)
            .WithMany(s => s.AulasAgendadas)
            .HasForeignKey(a => a.SalaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para acelerar a checagem de conflito de horário (sala e professor)
        // feita em CronogramaService — a sobreposição em si é validada em memória,
        // pois depende de comparação de intervalos (HoraInicio/HoraFim).
        builder.HasIndex(a => new { a.SalaId, a.DiaSemana });
        builder.HasIndex(a => new { a.ProfessorId, a.DiaSemana });
        builder.HasIndex(a => new { a.TurmaId, a.DiaSemana });
    }
}
