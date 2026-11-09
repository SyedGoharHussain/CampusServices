using Campus.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Campus.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _users;
    private readonly TokenService _tokens;

    public AuthController(UserManager<IdentityUser> users, TokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(CredentialsDto dto)
    {
        var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
        var result = await _users.CreateAsync(user, dto.Password);   // password is hashed here
        if (!result.Succeeded) return BadRequest(result.Errors.Select(e => e.Description));
        await _users.AddToRoleAsync(user, "Student");                // always Student, never from client
        return Ok();
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(CredentialsDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user == null || !await _users.CheckPasswordAsync(user, dto.Password))
            return Unauthorized(new { error = "Wrong email or password" });

        var role = (await _users.GetRolesAsync(user)).First();
        return Ok(new { Token = _tokens.Create(user, role), Role = role });
    }
}
