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
    public async Task<IActionResult> Login([FromBody] string login)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            return BadRequest("Le login est obligatoire.");
        }

        var client = _httpClientFactory.CreateClient("GestionOrdinateursAPI");
        var response = await client.GetAsync($"api/Users/login/{login}");

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

        // 2. Création des Claims (Revendications)
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        // 3. Clé secrète de signature (à adapter ou mettre dans appsettings.json)
        var secretKey = _configuration["Jwt:Key"] ?? "MaCleeSuperSecreteAChangerAbsolumentPourLeTP5_2026!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 4. Génération du token JWT valide 15 minutes
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