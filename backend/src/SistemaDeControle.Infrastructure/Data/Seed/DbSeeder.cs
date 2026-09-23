using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Infrastructure.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, string? adminEmail, string? adminPassword)
    {
        await context.Database.MigrateAsync();

        if (!await context.Usuarios.AnyAsync())
        {
            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
                throw new InvalidOperationException(
                    "Banco sem usuários: defina 'Seed:AdminEmail' e 'Seed:AdminPassword' (ADMIN_EMAIL/ADMIN_PASSWORD no .env) para criar o usuário inicial.");

            var hasher = new PasswordHasher<Usuario>();
            var usuario = new Usuario
            {
                Nome = "Administrador",
                Email = adminEmail,
            };
            usuario.SenhaHash = hasher.HashPassword(usuario, adminPassword);

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
        }
    }
}
