using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Domain.Entities;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.Locations;
public static class CreateLocation
{
    public record Command(string Name, Guid RegionId) : IRequest<Guid>;
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la localización es obligatorio.");
        }
    }
    public class Handler : IRequestHandler<Command, Guid>
    {
        private readonly InventoryDbContext _dbContext;
        public Handler(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var region = await _dbContext.Regions.FindAsync(new object[] { request.RegionId }, cancellationToken);
            if(region == null)
            {
                throw new Exception("La región ingresada no existe");
            }

            var newLocation = new Location(Guid.NewGuid(), request.Name, request.RegionId);

            _dbContext.Locations.Add(newLocation);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return newLocation.Id;
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapPost("/locations", async (Command command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .WithName("Create Location")
            .WithTags("Inventory - Locations");
        }
    }
}