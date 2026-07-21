using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Identity.Database;
using PIMS_MS.Modules.Identity.Features._EndpointGroup;

namespace PIMS_MS.Modules.Identity.Features.Auth;

public static class LoginWithCredentials
{
    public record Command(string Email, string Password) : IRequest<LoginResponse>;
    public record LoginResponse(string Token, string RefreshToken, DateTime Expiration);

    public class Handler : IRequestHandler<Command, LoginResponse>
    {
        private readonly IdentityDbContext _dbContext;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher _passwordHasher;

        public Handler(IdentityDbContext dbContext, IJwtTokenGenerator jwtTokenGenerator, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas. Correo electrónico o contraseña incorrectos.");
            }
            var expiration = DateTime.UtcNow.AddMinutes(30);
            
            string token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.Role, user.LocationId);
            return new LoginResponse(token, string.Empty, expiration);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapIdentityGroup().MapPost("/login", async (Command command, IMediator mediator) =>
                {
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                })
                .WithName("Login")
                .AllowAnonymous();
        }
    }
}