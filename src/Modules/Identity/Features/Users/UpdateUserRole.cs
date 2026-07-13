using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Identity.Database;
using PIMS_MS.Modules.Identity.Features._EndpointGroup;

namespace PIMS_MS.Modules.Identity.Features.Users;

public static class UpdateUserRole
{
    public record Command(Guid UserId, string NewRole) : IRequest;
    public class Handler : IRequestHandler<Command>
    {
        private readonly IdentityDbContext _dbContext;

        public Handler(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.UpdateRole(request.NewRole);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.NewRole).NotEmpty();
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapIdentityGroup().MapPut("/users/{userId:guid}/role", async (Guid userId, Command command, IMediator mediator) =>
                {
                    if (userId != command.UserId)
                    {
                        return Results.BadRequest("User ID in the URL does not match the User ID in the request body.");
                    }

                    await mediator.Send(command);
                    return Results.NoContent();
                })
                .WithName("UpdateUserRole");
        }
    }
}