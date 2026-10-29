using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.API.Events;

namespace GestionOrdinateurs.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdinateurController : ControllerBase
{
    private readonly IOrdinateurService _service;
    private readonly ILogger<OrdinateurController> _logger;
    private readonly IConfiguration _configuration;

    public OrdinateurController(IOrdinateurService service, ILogger<OrdinateurController> logger, IConfiguration configuration)
    {
        _service = service;
        _logger = logger;
        _configuration = configuration;

        _service.OrdinateurAjoute += AfficherMessageOrdinateurAjoute;
        _service.OrdinateurMisAJour += AfficherMessageOrdinateurMisAJour;
        _service.OrdinateurSupprime += AfficherMessageOrdinateurSupprime;
    }

    private void AfficherMessageOrdinateurAjoute(object? sender, OrdinateurAjouteEventArgs e)
    {
        _logger.LogInformation("Un nouvel ordinateur a été ajouté.");
    }

    private void AfficherMessageOrdinateurMisAJour(object? sender, OrdinateurMisAJourEventArgs e)
    {
        _logger.LogInformation("L'ordinateur ID {Id} a été mis à jour.", e.Ordinateur.Id);
    }

    private void AfficherMessageOrdinateurSupprime(object? sender, OrdinateurSupprimeEventArgs e)
    {
        _logger.LogInformation("L'ordinateur ID {Id} a été supprimé du parc.", e.OrdinateurId);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Ordinateur>>> GetAll()
    {
        _logger.LogInformation("Récupération de la liste des ordinateurs.");
        var liste = await _service.GetAllAsync();
        return Ok(liste);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Ordinateur>> GetById(int id)
    {
        _logger.LogInformation("Recherche de l'ordinateur avec l'ID {Id}", id);
        var ordi = await _service.GetByIdAsync(id);
        if (ordi is null)
            return NotFound($"Ordinateur {id} introuvable.");

        return Ok(ordi);
    }

    [HttpGet("marque/{marque}")]
    public async Task<ActionResult<List<Ordinateur>>> GetByMarque(string marque)
    {
        _logger.LogInformation("Recherche des ordinateurs de la marque : {Marque}", marque);
        var resultats = await _service.GetByMarqueAsync(marque);
        return Ok(resultats);
    }

    [HttpGet("disponibles")]
    public async Task<ActionResult<List<Ordinateur>>> GetDisponibles()
    {
        _logger.LogInformation("Récupération de la liste des ordinateurs disponibles");
        var disponibles = await _service.GetDisponiblesAsync();
        return Ok(disponibles);
    }

    [HttpGet("tri_prix")]
    public async Task<ActionResult<List<Ordinateur>>> GetTriParPrixDecroissant()
    {
        _logger.LogInformation("Récupération de la liste des ordinateurs triés du plus cher au moins cher.");
        var ordinateursTries = await _service.GetTriPrixDecroissantAsync();
        return Ok(ordinateursTries);
    }

    [HttpGet("{id}/diagnostic")]
    public async Task<ActionResult<string>> GetDiagnostic(int id)
    {
        var ordinateur = await _service.GetByIdAsync(id);
        if (ordinateur is null)
            return NotFound($"Aucun ordinateur trouvé avec l'ID {id}.");

        var diagnostic = _service.Diagnostiquer(ordinateur);
        return Ok(new { OrdinateurId = id, Nom = ordinateur.Nom, Diagnostic = diagnostic });
    }

    [HttpGet("statistiques")]
    public IActionResult GetStatistiques()
    {
        var (total, enStock, valeurTotal, totalReparations) = _service.CalculerStatistiques();
        return Ok(new
        {
            TotalOrdinateurs = total,
            OrdinateursDisponibles = enStock,
            ValeurTotaleStock = valeurTotal,
            TotalReparations = totalReparations
        });
    }

    [HttpGet("inventaire/nom/{nom}")]
    public async Task<IActionResult> ChercherParNom(string nom)
    {
        var list = await _service.GetAllAsync();
        var inventaire = new InventaireOrdinateurs { Ordinateurs = list };
        var ordinateur = inventaire[nom];

        if (ordinateur is null)
            return NotFound($"Aucun ordinateur trouvé avec le nom '{nom}'.");

        return Ok(ordinateur);
    }

    [HttpGet("inventaire/id/{id}")]
    public async Task<IActionResult> ChercherParId(int id)
    {
        var list = await _service.GetAllAsync();
        var inventaire = new InventaireOrdinateurs { Ordinateurs = list };
        var ordinateur = inventaire[id];

        if (ordinateur is null)
            return NotFound($"Aucun ordinateur trouvé avec l'ID {id}.");

        return Ok(ordinateur);
    }

    [HttpGet("info")]
    public IActionResult GetAppInfo()
    {
        var appName = _configuration["Application:Name"];
        var appVersion = _configuration["Application:Version"];

        return Ok(new 
        {
            NomApplication = appName,
            VersionApplication = appVersion
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Ordinateur>> Add([FromBody] Ordinateur ordinateur)
    {
        _logger.LogInformation("Ajout d'un nouvel ordinateur : {Nom}", ordinateur.Nom);
        await _service.AddAsync(ordinateur);
        return CreatedAtAction(nameof(GetById), new { id = ordinateur.Id }, ordinateur);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] Ordinateur ordinateur)
    {
        _logger.LogInformation("Mise à jour de l'ordinateur avec l'ID : {Id}", id);
        var existant = await _service.GetByIdAsync(id);
        if (existant is null)
            return NotFound($"Ordinateur {id} introuvable.");

        await _service.UpdateAsync(id, ordinateur);
        return Ok(ordinateur);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Suppression de l'ordinateur avec l'ID : {Id}", id);
        var ordi = await _service.GetByIdAsync(id);
        if (ordi is null)
            return NotFound($"Ordinateur {id} introuvable.");

        await _service.DeleteAsync(id);
        return NoContent();
    }
}