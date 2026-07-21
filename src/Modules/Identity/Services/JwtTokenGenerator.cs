using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PIMS_MS.Api.Modules.Identity.Domain.Entities;
using PIMS_MS.Common.Interfaces;

namespace PIMS_MS.Modules.Identity.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Guid UserId, string Email, string Role, Guid LocationId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
            new Claim(ClaimTypes.Email, Email),
            new Claim(ClaimTypes.Role, Role),
            new Claim("role", Role),
            new Claim("location_id", LocationId.ToString())
        };

        var secret = _configuration["JwtSettings:Secret"] 
                    ?? throw new InvalidOperationException("El secreto JWT (JwtSettings:Secret) no está configurado en appsettings.json o en las variables de entorno.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}