using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;

namespace PIMS_MS.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger, 
        IHttpContextAccessor httpContextAccessor)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();
        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        // Si tarda más de 500ms, disparamos una alerta de rendimiento (Warning)
        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anónimo";

            _logger.LogWarning(
                "⚠️ [CUELLO DE BOTELLA] {RequestName} tardó {ElapsedMilliseconds}ms | Usuario: {UserId} | Payload: {@Request}", 
                requestName, elapsedMilliseconds, userId, request);
        }

        return response;
    }
}