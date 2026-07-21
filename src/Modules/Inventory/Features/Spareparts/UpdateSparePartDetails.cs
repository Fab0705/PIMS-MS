using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.Spareparts;

public static class UpdateSparePartDetails
{
    public record Command(Guid SparePartId, string Description, bool IsRework) : IRequest;
    public class Handler : IRequestHandler<Command>
    {
        private readonly InventoryDbContext _dbContext;

        public Handler(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var sparePart = await _dbContext.SpareParts.FindAsync(new object[] { request.SparePartId }, cancellationToken);
            if (sparePart == null)
            {
                throw new Exception("Repuesto no encontrado.");
            }

            sparePart.UpdateDetails(request.Description, request.IsRework);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Description).NotEmpty().WithMessage("La descripción del repuesto es obligatoria.");
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapPut("/api/spareparts/{sparePartId:guid}/details", async (Guid sparePartId, Command command, IMediator mediator) =>
            {
                if (sparePartId != command.SparePartId)
                {
                    return Results.BadRequest("El ID del repuesto en la URL no coincide con el ID en el cuerpo de la solicitud.");
                }

                await mediator.Send(command);
                return Results.NoContent();
            })
            .WithName("UpdateSparePartDetails");
        }
    }
}