using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaDeControle.Domain.Entities;

namespace SistemaDeControle.Infrastructure.Data.Seed;

public enum ResultadoSeedAdmin
{
    Inalterado,
    Criado,
    SenhaRedefinida,
}

public static class DbSeeder
{
    /// <summary>
    /// Aplica as migrations e garante que a conta administradora configurada exista.
    /// Se <paramref name="redefinirSenhaAdmin"/> for true, a senha dessa conta volta a ser
    /// <paramref name="adminPassword"/> — recuperação de acesso quando a senha é esquecida.
    /// </summary>
    public static async Task<ResultadoSeedAdmin> SeedAsync(
        AppDbContext context, string? adminEmail, string? adminPassword, bool redefinirSenhaAdmin = false)
    {
        await context.Database.MigrateAsync();

        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            if (!await context.Usuarios.AnyAsync())
                throw new InvalidOperationException(
                    "Banco sem usuários: defina 'Seed:AdminEmail' e 'Seed:AdminPassword' (ADMIN_EMAIL/ADMIN_PASSWORD no .env) para criar o usuário inicial.");

            return ResultadoSeedAdmin.Inalterado;
        }

        var hasher = new PasswordHasher<Usuario>();
        var admin = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (admin is null)
        {
            var usuario = new Usuario { Nome = "Administrador", Email = adminEmail };
            usuario.SenhaHash = hasher.HashPassword(usuario, ExigirSenha(adminPassword, "criar a conta administradora"));
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
            return ResultadoSeedAdmin.Criado;
        }

        if (redefinirSenhaAdmin)
        {
            admin.SenhaHash = hasher.HashPassword(admin, ExigirSenha(adminPassword, "redefinir a senha do administrador"));
            await context.SaveChangesAsync();
            return ResultadoSeedAdmin.SenhaRedefinida;
        }

        return ResultadoSeedAdmin.Inalterado;
    }

    private static string ExigirSenha(string? senha, string acao) =>
        string.IsNullOrWhiteSpace(senha)
            ? throw new InvalidOperationException($"Defina 'Seed:AdminPassword' (ADMIN_PASSWORD no .env) para {acao}.")
            : senha;
}
