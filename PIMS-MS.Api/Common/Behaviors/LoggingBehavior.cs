using MediatR;
using System.Diagnostics;
using System.Security.Claims;

namespace PIMS_MS.Api.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger, 
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        // Extraemos el ID del usuario desde el JWT (si no hay, es anónimo)
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anónimo";

        _logger.LogInformation("🚀 [INICIO] Ejecutando {RequestName} | Usuario: {UserId}", requestName, userId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Pasa al siguiente behavior o al Handler real
            var response = await next();
            
            stopwatch.Stop();
            _logger.LogInformation("✅ [ÉXITO] {RequestName} completado en {ElapsedMilliseconds}ms", requestName, stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            // Logueamos el error EXACTAMENTE donde ocurrió antes de que explote la app
            _logger.LogError(ex, "❌ [ERROR] {RequestName} falló tras {ElapsedMilliseconds}ms. Motivo: {Message}", requestName, stopwatch.ElapsedMilliseconds, ex.Message);
            throw; 
        }
    }
}