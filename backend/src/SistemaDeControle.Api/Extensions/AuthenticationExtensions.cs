using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SistemaDeControle.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Configuração 'Jwt:Secret' não definida.");

        // HMAC-SHA256 exige chave de pelo menos 256 bits; recusa subir com chave fraca.
        if (Encoding.UTF8.GetByteCount(secret) < 32)
            throw new InvalidOperationException("Configuração 'Jwt:Secret' deve ter no mínimo 32 caracteres.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ClockSkew = TimeSpan.FromMinutes(2),
                };
            });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        return services;
    }
}
