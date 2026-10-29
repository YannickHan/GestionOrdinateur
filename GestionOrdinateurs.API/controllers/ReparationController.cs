namespace GestionOrdinateurs.API;

using GestionOrdinateurs.API.services.interfaces;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/reparations")]
public sealed class ReparationController : ControllerBase
{
    private readonly IOrdinateurService _ordinateurService;
    private readonly IReparationService _reparationService;
    private readonly StockService _stockService;

    // Injecte les services utilisés par les endpoints de réparation et de stock.
    public ReparationController(
        IOrdinateurService ordinateurService,
        IReparationService reparationService,
        StockService stockService)
    {
        _ordinateurService = ordinateurService;
        _reparationService = reparationService;
        _stockService = stockService;
    }

    // Lance en parallèle la réparation des trois premiers ordinateurs.
    [HttpPost("simuler")]
    public async Task<IActionResult> SimulerReparation()
    {
        IReadOnlyList<Ordinateur> ordinateurs = _ordinateurService.GetAll();

        await Task.WhenAll(
            _reparationService.SimulerReparationAsync(ordinateurs[0]),
            _reparationService.SimulerReparationAsync(ordinateurs[1]),
            _reparationService.SimulerReparationAsync(ordinateurs[2]));

        return Accepted();
    }

    // Affiche les réparations avec les informations des ordinateurs associés.
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReparations()
    {
        IReadOnlyList<Reparation> reparations =
            await _reparationService.GetAllAsync();

        return Ok(reparations);
    }

    [HttpGet("historique")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetHistorique()
    {
        IReadOnlyList<HistoriqueReparationRecord> historique =
            await _reparationService.GetHistoriqueAsync();

        return Ok(historique);
    }

    // Consomme dix SSD en parallèle et renvoie le stock restant.
    [HttpGet("stock-ssd")]
    [Authorize]
    public IActionResult GetStockSSD()
    {
        Parallel.For(0, 10, _ => _stockService.ConsommerSSD());
        return Ok(_stockService.StockSSD);
    }
}
