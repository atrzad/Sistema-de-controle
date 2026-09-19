using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SistemaDeControle.Application.Interfaces;
using SistemaDeControle.Application.Services;

namespace SistemaDeControle.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProfessorService, ProfessorService>();
        services.AddScoped<ITurmaService, TurmaService>();
        services.AddScoped<ISalaService, SalaService>();
        services.AddScoped<ICronogramaService, CronogramaService>();
        services.AddScoped<IFrequenciaService, FrequenciaService>();
        services.AddScoped<IRelatorioService, RelatorioService>();
        services.AddScoped<IJustificativaService, JustificativaService>();

        return services;
    }
}
