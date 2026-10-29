namespace AuthentificationApi.Controllers;

using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Net;

[ApiController]
[Route("auth")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthenticationController(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Login))
        {
            return BadRequest("Le login est obligatoire.");
        }

        var usersClient = _httpClientFactory.CreateClient("UsersApi");
        using var userResponse = await usersClient.GetAsync(
            $"api/Users/{Uri.EscapeDataString(request.Login)}",
            cancellationToken);

        if (userResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return Unauthorized("Ce login n'existe pas.");
        }

        if (!userResponse.IsSuccessStatusCode)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                "L'API des utilisateurs est indisponible.");
        }

        var user = await userResponse.Content.ReadFromJsonAsync<UserInfo>(
            cancellationToken);

        if (user is null)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                "La réponse de l'API des utilisateurs est invalide.");
        }

        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            return Problem("La clé JWT n'est pas configurée.");
        }

        var key = Encoding.UTF8.GetBytes(jwtKey);
        return Ok(new
        {
            token = GenTokenJWT(key, user),
            expiresAt = DateTime.UtcNow.AddMinutes(15)
        });
    }

    private string GenTokenJWT(byte[] keyP, UserInfo user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyP), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);
        return jwt;
    }
}

public sealed record LoginRequest(string Login);

internal sealed record UserInfo(int Id, string Login, string Role);