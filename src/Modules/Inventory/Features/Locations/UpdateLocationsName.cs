using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.Locations;

public static class UpdateLocationsName
{
    public record Command(Guid Id, string NewName) : IRequest;
    public record UpdateLocationRequest(string NewName);
    public class Handler : IRequestHandler<Command>
    {
        private readonly InventoryDbContext _dbContext;
        public Handler(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var location = await _dbContext.Locations.FindAsync(new object[] { request.Id }, cancellationToken);
            
            if(location == null)
            {
                throw new Exception("La locación ingresada no existe");
            }

            location.UpdateName(request.NewName);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapPut("/locations/{id:guid}", async (
                Guid id, 
                [FromBody] UpdateLocationRequest request, 
                ISender sender) =>
            {
                var command = new Command(id, request.NewName);
                
                await sender.Send(command);
                
                return Results.NoContent(); 
            })
            .WithName("Update Location Name")
            .WithTags("Inventory - Locations");
        }
    }
}