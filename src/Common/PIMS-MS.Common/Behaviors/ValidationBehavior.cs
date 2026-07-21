using FluentValidation;
using MediatR;

namespace PIMS_MS.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull
{
    // Inyecta TODOS los validadores que existan para este TRequest específico
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            // Ejecuta todos los validadores en paralelo por rendimiento
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Recolecta todos los errores (ej: "El nombre está vacío", "El stock no puede ser negativo")
            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                // ¡Falla rápido! El Handler de BD nunca se llega a ejecutar.
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}