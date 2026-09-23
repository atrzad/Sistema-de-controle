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

        // Concorrência otimista via coluna de sistema xmin do Postgres (não gera coluna nova):
        // um UPDATE/DELETE sobre uma linha alterada por outra transação desde a leitura
        // lança DbUpdateConcurrencyException (mapeada para 409).
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(Domain.Common.BaseEntity).IsAssignableFrom(t.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType).Property<uint>("xmin").IsRowVersion();
        }

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
        var agora = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = agora;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = agora;
            }
        }
    }
}
