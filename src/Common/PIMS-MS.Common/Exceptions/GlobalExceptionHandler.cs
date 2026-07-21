using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PIMS_MS.Common.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "❌ Ocurrió un error: {Message}", exception.Message);

        // 1. Configuramos el formato base estándar (ProblemDetails)
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Status = StatusCodes.Status500InternalServerError,
            Title = "Error Interno del Servidor",
            Detail = "Ha ocurrido un error inesperado. Por favor, contacte a soporte técnico."
        };

        // 2. Mapeo de Excepciones a Códigos HTTP
        if (exception is ValidationException validationException)
        {
            // Error 400: Falló el ValidationBehavior
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Error de Validación";
            problemDetails.Detail = "Uno o más campos tienen errores.";
            
            // Agrupamos los errores de FluentValidation para el Frontend
            problemDetails.Extensions["errors"] = validationException.Errors
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
        else if (exception is NotFoundException || exception is KeyNotFoundException)
        {
            // Error 404: No se encontró el recurso en la BD
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Title = "Recurso no encontrado";
            problemDetails.Detail = exception.Message;
        }
        else if (exception is UnauthorizedAccessException)
        {
            problemDetails.Status = StatusCodes.Status401Unauthorized;
            problemDetails.Title = "Acceso Denegado";
            problemDetails.Detail = "No tienes permisos para realizar esta acción.";
        }
        else if (exception is DomainException || exception is ArgumentException || exception is InvalidOperationException)
        {
            // Error 400: Regla de negocio rota en el Dominio
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Regla de Negocio Inválida";
            problemDetails.Detail = exception.Message;
        }
        else if (exception is KeyNotFoundException) // O tu propia NotFoundException
        {
            // Error 404: No se encontró el recurso en la BD
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Title = "Recurso no encontrado";
            problemDetails.Detail = exception.Message;
        }

        // 3. Escribimos la respuesta HTTP
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Devolvemos 'true' para decirle a .NET: "Ya manejé este error, no rompas la app"
        return true; 
    }
}