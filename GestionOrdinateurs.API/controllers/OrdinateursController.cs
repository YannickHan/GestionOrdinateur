namespace GestionOrdinateurs.API;

using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.services.interfaces;
using GestionOrdinateurs.API.contexts;

// Indique que cette classe est un contrôleur d'API.
[ApiController]
// La route de base devient /api/Ordinateurs.
[Route("api/[controller]")]
public class OrdinateursController : ControllerBase
{
    private readonly IOrdinateurService _Service;
    private readonly ILogger<OrdinateursController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ParcInformatiqueContext _context;

    // Reçoit le service qui contient la logique de gestion des ordinateurs.
    public OrdinateursController(IOrdinateurService Service, ILogger<OrdinateursController> logger, IConfiguration configuration, ParcInformatiqueContext context)
    {
        _Service = Service;
        _logger = logger;
        _configuration = configuration;
        _context = context;
    }

    // Ajoute un nouvel ordinateur.
    // Route : POST /api/Ordinateurs
    [HttpPost]
    public IActionResult AddOrdinateur(int id, string nom, string marque, int ramGo, int stockageGo, TypeOrdinateur typeOrdi, string processeur, EtatOrdinateur etat, decimal prix = 0, int stock = 0)
    {
        if (id <= 0 ||
            string.IsNullOrWhiteSpace(nom) ||
            string.IsNullOrWhiteSpace(marque) ||
            prix < 0 ||
            stock < 0 ||
            ramGo <= 0 ||
            stockageGo < 0 ||
            string.IsNullOrWhiteSpace(processeur))
        {
            _logger.LogWarning("Données invalides pour l'ajout de l'ordinateur : {Id}, {Nom}", id, nom);
            return BadRequest("Les informations de l'ordinateur sont invalides.");
        }

        if (!_Service.Add(id, nom, marque, ramGo, stockageGo, typeOrdi, processeur, etat, prix, stock))
        {
            _logger.LogWarning("Un ordinateur avec l'identifiant {Id} existe déjà.", id);
            return Conflict($"Un ordinateur avec l'identifiant {id} existe déjà.");
        }

        _logger.LogInformation("Ordinateur ajouté : {Id}, {Nom}", id, nom);
        return CreatedAtAction(nameof(GetOrdinateurById), new { id }, null);
    }

    // Supprime un ordinateur grâce à son identifiant.
    // Route : DELETE /api/Ordinateurs/{id}
    [HttpDelete("{id}")]
    public IActionResult RemoveOrdinateur(int id)
    {
        if(_Service.Remove(id))
        {
            _logger.LogInformation($"Ordinateur supprimé : {id}");
            return NoContent();
        }
        return NotFound();
    }

    // Modifie les informations d'un ordinateur existant.
    // Route : PUT /api/Ordinateurs/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateOrdinateur(int id, string nom, int ramGo, int stockageGo, TypeOrdinateur typeOrdi, string processeur, EtatOrdinateur etat, decimal prix = 0, int stock = 0)
    {
        if (ramGo <= 0 ||
            stockageGo < 0 ||
            string.IsNullOrWhiteSpace(processeur) ||
            prix < 0 ||
            stock < 0)
        {
            return BadRequest("Les informations de l'ordinateur sont invalides.");
        }

        if(_Service.Update(id, nom, ramGo, stockageGo, typeOrdi, processeur, etat, prix, stock))
        {
            _logger.LogInformation($"Ordinateur mis à jour : {id}");
            return Ok();
        }
        return NotFound();
    }

    // Récupère la liste de tous les ordinateurs.
    // Route : GET /api/Ordinateurs
    [HttpGet]
    public IActionResult GetAllOrdinateurs()
    {
        _logger.LogInformation("Récupération de tous les ordinateurs");
        IReadOnlyList<Ordinateur> ordinateurs = _Service.GetAll();
        return Ok(ordinateurs);
    }

    // Récupère un ordinateur grâce à son identifiant.
    // Route : GET /api/Ordinateurs/{id}
    [HttpGet("{id}")]
    public IActionResult GetOrdinateurById(int id)
    {
        _logger.LogInformation($"Récupération de l'ordinateur : {id}");
        IReadOnlyList<Ordinateur> ordinateurs = _Service.GetById(id);
        if (ordinateurs == null)
        {
            return NotFound();
        }
        return Ok(ordinateurs);
    }

    [HttpGet("{id}/diagnostic")]
    public IActionResult GetDiagnostic(int id)
    {
        Ordinateur? ordinateur = _context.Ordinateurs.FirstOrDefault(o => o.Id == id);
        if (ordinateur is null)
        {
            return NotFound();
        }

        string diagnostic = _Service.Diagnostiquer(ordinateur);
        return Ok(diagnostic);
    }

    [HttpGet("statistiques")]
    public IActionResult GetStatistiques()
    {
        var (total, enStock, valeurStock, nbReparations) =
            _Service.CalculerStatistiques();

        return Ok(new { total, enStock, valeurStock, nbReparations });
    }

    [HttpGet("inventaire/{nom}")]
    public IActionResult ChercherDansInventaire(string nom)
    {
        InventaireOrdinateurs inventaire = new()
        {
            Ordinateurs = _context.Ordinateurs.ToList()
        };

        Ordinateur? ordinateur = inventaire[nom];
        return ordinateur is null ? NotFound() : Ok(ordinateur);
    }

    // Recherche les ordinateurs correspondant à une marque.
    // Route : GET /api/Ordinateurs/marque/{marque}
    [HttpGet("marque/{marque}")]
    public IActionResult GetOrdinateursByMarque(string marque)
    {
        _logger.LogInformation($"Recherche des ordinateurs de marque : {marque}");
        IReadOnlyList<Ordinateur> ordinateurs = _Service.GetByMarque(marque);
        return Ok(ordinateurs);
    }

    // Récupère les informations de l'application (nom et version).
    // Route : GET /api/Ordinateurs/info
    [HttpGet("ordinateurs/info")]
    public IActionResult GetApplicationInfo()
    {
        var appName = _configuration["Application:Name"];
        var appVersion = _configuration["Application:Version"];

        return Ok(new { Name = appName, Version = appVersion });
    }

    // Récupère la liste des ordinateurs disponibles (stock > 0).
    // Route : GET /api/Ordinateurs/available
    [HttpGet("available")]
    public IActionResult GetAvailableOrdinateurs()
    {
        _logger.LogInformation("Récupération des ordinateurs disponibles");
        IReadOnlyList<Ordinateur> ordinateurs = _Service.GetAll();
        var availableOrdinateurs = ordinateurs.Where(o => o.Stock > 0).ToList();
        return Ok(availableOrdinateurs);
    }

    // Récupère la liste des ordinateurs triés par prix croissant.
    // Route : GET /api/Ordinateurs/price-croissant
    [HttpGet("price-croissant")]
    public IActionResult GetOrdinateursPriceCroissant()
    {
        _logger.LogInformation("Récupération des ordinateurs triés par prix croissant");
        IReadOnlyList<Ordinateur> ordinateurs = _Service.GetAll();
        var sortedOrdinateurs = ordinateurs.OrderBy(o => o.Prix).ToList();
        return Ok(sortedOrdinateurs);
    }
}
