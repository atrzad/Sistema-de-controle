using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Professor> Professores => Set<Professor>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<Sala> Salas => Set<Sala>();
    public DbSet<AulaAgendada> AulasAgendadas => Set<AulaAgendada>();
    public DbSet<RegistroFrequencia> RegistrosFrequencia => Set<RegistroFrequencia>();
    public DbSet<Justificativa> Justificativas => Set<Justificativa>();
    public DbSet<Anexo> Anexos => Set<Anexo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        AtualizarTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AtualizarTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AtualizarTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
