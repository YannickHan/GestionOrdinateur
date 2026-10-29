namespace GestionOrdinateurs.API;

using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.services.interfaces;
using GestionOrdinateurs.API.contexts;
using GestionOrdinateurs.API.events;

//toute les fonctions dans cette classe sont des fonctions qui vont appeler les fonctions de la classe ParcInitializer.cs
public class OrdinateurService : IOrdinateurService
{
    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAdded;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurRemoved;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurUpdated;
    private readonly ParcInformatiqueContext _context;
    private readonly JournalService _journalService;

    public OrdinateurService(ParcInformatiqueContext context, JournalService? journalService = null)
    {
        _context = context;
        _journalService = journalService ?? new JournalService();

        // Abonnement aux événements pour afficher les messages dans la console
        OrdinateurAdded += AfficherMessage;
        OrdinateurRemoved += AfficherMessage;
        OrdinateurUpdated += AfficherMessage;
    }

    // Méthode pour ajouter un ordinateur
    public bool Add(int id, string nom, string marque, int ramGo, int stockageGo, TypeOrdinateur typeOrdi, string processeur, EtatOrdinateur etat, decimal prix = 0, int stock = 0)
    {
        if (_context.Ordinateurs
            .Any(o => o.Id == id))
        {
            return false; // L'ordinateur avec cet ID existe déjà
        }
        Caracteristiques caracteristiques = new(ramGo, stockageGo, typeOrdi, processeur);
        Ordinateur ordinateur = new(id, nom, marque, etat, prix, stock, caracteristiques);
        _context.Ordinateurs.Add(ordinateur);
        _context.SaveChanges();
        _journalService.EcrireOrdinateur($"{ordinateur.Nom} ajouté");
        OrdinateurAdded?.Invoke(this, new OrdinateurAjouteEventArgs(ordinateur));
        return true;
    }

    // Méthode pour supprimer un ordinateur par son ID
    public bool Remove(int id)
    {
        Ordinateur? ordinateur = _context.Ordinateurs
            .FirstOrDefault(o => o.Id == id);
        if (ordinateur == null)
        {
            return false;
        }

        _context.Ordinateurs.Remove(ordinateur);
        _context.SaveChanges();
        _journalService.EcrireOrdinateur($"{ordinateur.Nom} supprimé");
        OrdinateurRemoved?.Invoke(this, new OrdinateurSupprimeEventArgs(ordinateur));
        return true;
    }

    // Méthode pour mettre à jour un ordinateur par son ID
    public bool Update(int id, string nom, int ramGo, int stockageGo, TypeOrdinateur typeOrdi, string processeur, EtatOrdinateur etat, decimal prix = 0, int stock = 0)
    {
        Ordinateur? ordinateur = _context.Ordinateurs
            .FirstOrDefault(o => o.Id == id);

        if (ordinateur == null)
        {
            return false;
        }

        ordinateur.Nom = nom;
        ordinateur.Caracteristiques = new Caracteristiques(ramGo, stockageGo, typeOrdi, processeur);
        ordinateur.Etat = etat;
        ordinateur.Prix = prix;
        ordinateur.Stock = stock;

        _context.SaveChanges();
        _journalService.EcrireOrdinateur($"{ordinateur.Nom} modifié");
        OrdinateurUpdated?.Invoke(this, new OrdinateurMisAJourEventArgs(ordinateur));
        return true;
    }

    // Méthode pour obtenir tous les ordinateurs
    public IReadOnlyList<Ordinateur> GetAll()
    {
        return _context.Ordinateurs
            .ToList()
            .AsReadOnly();
    }

    // Méthode pour obtenir un ordinateur par son ID
    public IReadOnlyList<Ordinateur> GetById(int id)
    {
        return _context.Ordinateurs
            .Where(o => o.Id == id)
            .ToList()
            .AsReadOnly();
    }

    // Méthode pour rechercher les ordinateurs par marque.
    public IReadOnlyList<Ordinateur> GetByMarque(string marque)
    {
        return _context.Ordinateurs
            .Where(o => o.Marque == marque)
            .ToList()
            .AsReadOnly();
    }

    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            { Etat: EtatOrdinateur.HorsService } => "Remplacement",
            { Etat: EtatOrdinateur.Lent, Caracteristiques: { RamGo: < 8 } } => "Ajouter RAM",
            _ => "Aucune action"
        };
    }

    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations)
        CalculerStatistiques()
    {
        List<Ordinateur> ordinateurs = _context.Ordinateurs.ToList();

        return (
            ordinateurs.Count,
            ordinateurs.Count(ordinateur => ordinateur.Stock > 0),
            ordinateurs.Sum(ordinateur => ordinateur.Prix * ordinateur.Stock),
            _context.Reparations.Count());
    }

    // Méthode pour afficher un message lorsqu'un ordinateur est ajouté
    private void AfficherMessage(object? sender, OrdinateurAjouteEventArgs e)
    {
        Console.WriteLine(
        $"Nouvel ordinateur : {e.Ordinateur.Nom}, Marque : {e.Ordinateur.Marque}, RAM : {e.Ordinateur.Caracteristiques?.RamGo} Go, Processeur : {e.Ordinateur.Caracteristiques?.Processeur}, État : {e.Ordinateur.Etat}");
    }

    // Méthode pour afficher un message lorsqu'un ordinateur est supprimé
    private void AfficherMessage(object? sender, OrdinateurSupprimeEventArgs e)
    {
        Console.WriteLine($"Ordinateur supprimé : {e.Ordinateur.Nom}, Marque : {e.Ordinateur.Marque}");
    }

    // Méthode pour afficher un message lorsqu'un ordinateur est mis à jour
    private void AfficherMessage(object? sender, OrdinateurMisAJourEventArgs e)
    {
        Console.WriteLine($"Ordinateur mis à jour : {e.Ordinateur.Nom}, Marque : {e.Ordinateur.Marque}, RAM : {e.Ordinateur.Caracteristiques?.RamGo} Go, Processeur : {e.Ordinateur.Caracteristiques?.Processeur}, État : {e.Ordinateur.Etat}");
    }
}