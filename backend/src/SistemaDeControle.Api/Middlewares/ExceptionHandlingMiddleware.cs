using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SistemaDeControle.Application.Common.Exceptions;

namespace SistemaDeControle.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Recurso não encontrado", exception.Message),
            ConflictException => (HttpStatusCode.Conflict, "Conflito", exception.Message),
            UnauthorizedAppException => (HttpStatusCode.Unauthorized, "Não autorizado", exception.Message),
            BadRequestAppException => (HttpStatusCode.BadRequest, "Requisição inválida", exception.Message),
            ValidationException => (HttpStatusCode.BadRequest, "Erro de validação", exception.Message),
            DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "Conflito",
                "O registro foi alterado ou removido por outra pessoa. Recarregue a página e tente novamente."),
            DbUpdateException { InnerException: PostgresException pg } => MapearErroPostgres(pg, exception),
            _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor",
                _environment.IsDevelopment() ? exception.Message : "Ocorreu um erro inesperado. Tente novamente mais tarde."),
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Erro não tratado ao processar {Path}", context.Request.Path);
        else
            _logger.LogWarning("{Title}: {Message}", title, exception.InnerException?.Message ?? exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
        };

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    // Violações de constraint que escapam das checagens prévias dos services
    // (ex.: duas requisições simultâneas) viram 409 em vez de 500.
    private (HttpStatusCode, string, string) MapearErroPostgres(PostgresException pg, Exception exception) => pg.SqlState switch
    {
        PostgresErrorCodes.UniqueViolation => (HttpStatusCode.Conflict, "Conflito",
            "Já existe um registro com esses dados."),
        PostgresErrorCodes.ExclusionViolation => (HttpStatusCode.Conflict, "Conflito",
            "Conflito de horário: a sala ou o professor já possui aula nesse intervalo."),
        PostgresErrorCodes.ForeignKeyViolation => (HttpStatusCode.Conflict, "Conflito",
            "O registro está vinculado a outros dados e não pode ser alterado ou removido."),
        _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor",
            _environment.IsDevelopment() ? exception.Message : "Ocorreu um erro inesperado. Tente novamente mais tarde."),
    };
}
