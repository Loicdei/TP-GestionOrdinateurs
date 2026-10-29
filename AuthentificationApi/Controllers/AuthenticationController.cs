using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using GestionOrdinateurs.Shared;

namespace AuthentificationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthenticationController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] JsonElement body)
    {
        string? login = null;

        // Supporte aussi bien une chaîne brute "user1" qu'un objet { "login": "user1" }
        if (body.ValueKind == JsonValueKind.String)
        {
            login = body.GetString();
        }
        else if (body.ValueKind == JsonValueKind.Object)
        {
            if (body.TryGetProperty("login", out var prop) || body.TryGetProperty("Login", out prop))
            {
                login = prop.GetString();
            }
        }

        if (string.IsNullOrWhiteSpace(login))
        {
            return BadRequest("Le login est obligatoire.");
        }

        var client = _httpClientFactory.CreateClient("GestionOrdinateursAPI");

        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync($"api/Users/login/{login}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur de communication avec GestionOrdinateurs.API : {ex.Message}");
        }

        if (!response.IsSuccessStatusCode)
        {
            return NotFound($"Utilisateur '{login}' introuvable dans l'API GestionOrdinateurs.");
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (user == null)
        {
            return Unauthorized("Impossible de récupérer les données utilisateur.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var secretKey = _configuration["Jwt:Key"] ?? "MaCleeSuperSecreteAChangerAbsolumentPourLeTP5_2026!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "AuthentificationApi",
            audience: _configuration["Jwt:Audience"] ?? "GestionOrdinateurs",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { Token = tokenString });
    }
}