namespace AuthentificationApi.Controllers;

using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Http.Json;

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

    [HttpPost("keycloak/login")]
    public async Task<IActionResult> KeycloakLogin(
        [FromBody] KeycloakLoginRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("L'utilisateur et le mot de passe sont obligatoires.");
        }

        var clientId = _configuration["Keycloak:ClientId"] ?? "gestion-ordinateurs-client";
        var realm = _configuration["Keycloak:Realm"] ?? "gestion-ordinateurs";
        var clientSecret = _configuration["Keycloak:ClientSecret"];
        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = clientId,
            ["username"] = request.Username,
            ["password"] = request.Password
        };

        if (!string.IsNullOrWhiteSpace(clientSecret))
        {
            form["client_secret"] = clientSecret;
        }

        var keycloakClient = _httpClientFactory.CreateClient("Keycloak");
        using var response = await keycloakClient.PostAsync(
            $"realms/{realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(form),
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Unauthorized("Utilisateur ou mot de passe incorrect.");
        }

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode(StatusCodes.Status502BadGateway,
                "Le service Keycloak est indisponible.");
        }

        var token = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(
            cancellationToken);
        return token is null
            ? StatusCode(StatusCodes.Status502BadGateway, "La réponse Keycloak est invalide.")
            : Ok(token);
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

public sealed record KeycloakLoginRequest(string Username, string Password);

internal sealed record KeycloakTokenResponse(
    [property: System.Text.Json.Serialization.JsonPropertyName("access_token")]
    string AccessToken,
    [property: System.Text.Json.Serialization.JsonPropertyName("expires_in")]
    int ExpiresIn);

internal sealed record UserInfo(int Id, string Login, string Role);