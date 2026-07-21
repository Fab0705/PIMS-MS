using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Domain.Entities;
using PIMS_MS.Modules.Inventory.Domain.Exceptions;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.Spareparts;

public static class RegisterSparePart
{
    public record Command(string PartNumber, string Description, bool IsRework, int Quantity) : IRequest<Result<SparePart>>;
    public record Result<T>(bool IsSuccess, T? Value, string? ErrorMessage = null);

    public class Handler : IRequestHandler<Command, Result<SparePart>>
    {
        private readonly ICurrentService _currentService;
        private readonly InventoryDbContext _dbContext;

        public Handler(InventoryDbContext dbContext, ICurrentService currentService)
        {
            _dbContext = dbContext;
            _currentService = currentService;
        }

        public async Task<Result<SparePart>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Check if a spare part with the same part number already exists
            var existingSparePart = await _dbContext.SpareParts.FirstOrDefaultAsync(sp => sp.PartNumber == request.PartNumber, cancellationToken);
            if (existingSparePart != null)
            {
                throw new DuplicatePartNumberException(request.PartNumber);
            }

            // Create a new SparePart entity
            var sparePart = new SparePart
            (
                Guid.NewGuid(),
                request.PartNumber,
                request.Description,
                request.IsRework
            );

            // Add the new spare part to the database
            _dbContext.SpareParts.Add(sparePart);

            var initialStock = new Stock
            (
                Guid.NewGuid(),
                sparePart.Id,
                _currentService.LocationId,
                request.Quantity
            );
            _dbContext.Stocks.Add(initialStock);
            
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new Result<SparePart>(true, sparePart);
        }
    }
    
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.PartNumber)
                .NotEmpty().WithMessage("El número de parte es obligatorio.")
                .MaximumLength(9).WithMessage("El número de parte no puede exceder los 9 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(200).WithMessage("La descripción no puede exceder los 200 caracteres.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad no puede ser negativa.");
        }
    }

    public class Endpint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapPost("/spareparts", async (Command command, ISender sender) =>
            {
                var result = await sender.Send(command);
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { error = result.ErrorMessage });
                }
                return Results.Ok(result.Value);
            })
            .WithName("RegisterSparePart");
        }
    }
}