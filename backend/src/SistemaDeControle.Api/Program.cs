using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using SistemaDeControle.Api.Extensions;
using SistemaDeControle.Api.Middlewares;
using SistemaDeControle.Application;
using SistemaDeControle.Infrastructure;
using SistemaDeControle.Infrastructure.Data;
using SistemaDeControle.Infrastructure.Data.Seed;

#if DESKTOP
var desktop = await SistemaDeControle.Api.Desktop.DesktopHost.IniciarAsync(args);
if (desktop is null)
    return;
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = SistemaDeControle.Api.Desktop.DesktopHost.FiltrarArgumentos(args),
    ContentRootPath = AppContext.BaseDirectory,
    EnvironmentName = Environments.Production,
});
desktop.ConfigurarBuilder(builder);
#else
var builder = WebApplication.CreateBuilder(args);
#endif

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Informe: Bearer {seu token}",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAppRateLimiting(builder.Configuration);
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

// A API roda atrás do nginx do frontend (e do Caddy em produção) e não é publicada
// diretamente no host, então confia nos cabeçalhos X-Forwarded-* recebidos pela rede interna.
// ForwardLimit = número de proxies na frente da API (1 = só nginx; 2 = Caddy + nginx).
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = builder.Configuration.GetValue("ForwardedHeaders:ForwardLimit", 1);
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var adminEmail = app.Configuration["Seed:AdminEmail"];
    var resultadoSeed = await DbSeeder.SeedAsync(
        db, adminEmail, app.Configuration["Seed:AdminPassword"], app.Configuration.GetValue("Seed:ResetAdminPassword", false));

    if (resultadoSeed == ResultadoSeedAdmin.Criado)
        app.Logger.LogInformation("Conta administradora {Email} criada.", adminEmail);
    else if (resultadoSeed == ResultadoSeedAdmin.SenhaRedefinida)
        app.Logger.LogWarning("Senha da conta {Email} redefinida. No servidor, volte RESET_ADMIN_PASSWORD para false.", adminEmail);
}

app.UseForwardedHeaders();
app.UseMiddleware<ExceptionHandlingMiddleware>();
#if DESKTOP
desktop.ConfigurarArquivosEstaticos(app);
#endif

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS é terminado no proxy reverso (Caddy); a API só fala HTTP na rede interna.
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();
#if DESKTOP
desktop.ConfigurarEndpoints(app);
#endif

app.Run();

public partial class Program { }
