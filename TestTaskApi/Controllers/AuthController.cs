using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestTaskApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var adminUsername = _configuration["ADMIN_USERNAME"];
        var adminPassword = _configuration["ADMIN_PASSWORD"];

        if (string.IsNullOrEmpty(adminUsername) ||
            string.IsNullOrEmpty(adminPassword))
        {
            throw new InvalidOperationException(
                "Admin credentials are not configured.");
        }

        if (request.Username != adminUsername ||
            request.Password != adminPassword)
        {
            return Unauthorized(new
            {
                message = "Invalid username or password."
            });
        }

        var jwtKey = _configuration["JWT_KEY"];

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException(
                "JWT_KEY is not configured.");
        }

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expiresInMinutes =
            _configuration.GetValue<int>("Jwt:ExpiresInMinutes");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, adminUsername)
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Ok(new
        {
            accessToken
        });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}