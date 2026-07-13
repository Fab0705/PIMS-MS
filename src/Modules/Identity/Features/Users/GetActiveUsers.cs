using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Identity.Database;
using PIMS_MS.Modules.Identity.Features._EndpointGroup;

namespace PIMS_MS.Modules.Identity.Features.Users;

public static class GetActiveUsers
{
    public record Query : IRequest<List<UsersResponse>>;
    public record UsersResponse(Guid Id, string Email, string Role, bool IsActive);
    public class Handler : IRequestHandler<Query, List<UsersResponse>>
    {
        private readonly IdentityDbContext _dbContext;

        public Handler(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UsersResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var activeUsers = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.IsActive)
                .Select(u => new UsersResponse(u.Id, u.Email, u.Role, u.IsActive))
                .ToListAsync(cancellationToken);

            return activeUsers;
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapIdentityGroup().MapGet("/users/active", async (IMediator mediator) =>
                {
                    var result = await mediator.Send(new Query());
                    return Results.Ok(result);
                })
                .WithName("GetActiveUsers");
        }
    }
}