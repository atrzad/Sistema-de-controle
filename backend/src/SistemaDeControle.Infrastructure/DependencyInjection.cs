using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Infrastructure.Auth;
using SistemaDeControle.Infrastructure.Data;
using SistemaDeControle.Infrastructure.Repositories;
using SistemaDeControle.Infrastructure.Storage;

namespace SistemaDeControle.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IProfessorRepository, ProfessorRepository>();
        services.AddScoped<ITurmaRepository, TurmaRepository>();
        services.AddScoped<ISalaRepository, SalaRepository>();
        services.AddScoped<ICronogramaRepository, CronogramaRepository>();
        services.AddScoped<IFrequenciaRepository, FrequenciaRepository>();
        services.AddScoped<IJustificativaRepository, JustificativaRepository>();

        services.AddSingleton<IArquivoStorageService, LocalFileStorageService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
