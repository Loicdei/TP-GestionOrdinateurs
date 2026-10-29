using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionOrdinateurs.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReparationController : ControllerBase
{
    private readonly ReparationService _reparService;
    private readonly IOrdinateurService _ordinateurService;
    private readonly StockService _stockService;

    public ReparationController(ReparationService reparService, IOrdinateurService ordinateurService, StockService stockService)
    {
        _reparService = reparService;
        _ordinateurService = ordinateurService;
        _stockService = stockService;
    }

    [HttpPost("SimulerReparation")]
    [AllowAnonymous]
    public async Task<IActionResult> SimulerReparation()
    {
        var listOrdinateurs = await _ordinateurService.GetAllAsync();

        if (listOrdinateurs.Count < 3)
        {
            return BadRequest("Il faut au moins 3 ordinateurs en base de données pour exécuter la simulation.");
        }

        await Task.WhenAll(
            _reparService.SimulerReparationAsync(listOrdinateurs[0]),
            _reparService.SimulerReparationAsync(listOrdinateurs[1]),
            _reparService.SimulerReparationAsync(listOrdinateurs[2])
        );

        return Ok("Simulation des réparations terminée.");
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Reparation>>> GetAll()
    {
        var reparations = await _reparService.GetAllAsync();
        return Ok(reparations);
    }

    [HttpGet("GetStockSSD")]
    public IActionResult GetStockSSD()
    {
        Parallel.For(
            0,
            10,
            i => _stockService.ConsommerSSD());

        Console.WriteLine($"Stock SSD restant : {_stockService.StockSSD}");

        return Ok(_stockService.StockSSD);
    }

    [HttpGet("historique")]
    public async Task<ActionResult<List<HistoriqueReparationRecord>>> GetHistorique()
    {
        var historique = await _reparService.GetHistoriqueAsync();
        return Ok(historique);
    }
}