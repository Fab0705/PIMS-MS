using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Api.Modules.Identity.Domain.Entities;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Identity.Database;
using PIMS_MS.Modules.Identity.Features._EndpointGroup;

namespace PIMS_MS.Modules.Identity.Features.Users;

public static class CreateUser
{
    public record Command(string Email, string Password, string Role) : IRequest<UserResponse>;
    public record UserResponse(Guid Id, string Email, string TemporaryPassword, string Message);

    public class Handler : IRequestHandler<Command, UserResponse>
    {
        private readonly IdentityDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public Handler(IdentityDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new Exception("User with this email already exists.");
            }

            var temporaryPassword = _passwordHasher.GenerateTemporaryPassword();
            var passwordHash = _passwordHasher.HashPassword(temporaryPassword);

            var newUser = new User(request.Email, passwordHash, request.Role);

            // Add the new user to the database
            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UserResponse(newUser.Id, newUser.Email, temporaryPassword, "Usuario creado correctamente, se ha enviado un correo electrónico con la contraseña temporal.");
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Role).NotEmpty();
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapIdentityGroup().MapPost("/create-user", async (Command command, IMediator mediator) =>
                {
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                })
                .WithName("CreateUser");
        }
    }
}