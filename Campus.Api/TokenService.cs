using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Campus.Api;

public class TokenService
{
    private readonly IConfiguration _config;
    public TokenService(IConfiguration config) { _config = config; }

    public string Create(IdentityUser user, string role)
    {
        var claims = new[]
        {
            new Claim("uid", user.Id),
            new Claim("email", user.Email!),
            new Claim("role", role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
