using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Infrastructure.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Usuarios.AnyAsync())
        {
            var hasher = new PasswordHasher<Usuario>();
            var usuario = new Usuario
            {
                Nome = "Administrador",
                Email = "pedagogo@sistemadecontrole.local",
            };
            usuario.SenhaHash = hasher.HashPassword(usuario, "TrocarSenha123!");

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
        }
    }
}
