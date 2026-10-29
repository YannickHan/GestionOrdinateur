namespace GestionOrdinateurs.Shared;

public static class ParcInitializer
{
    //Initialise la liste des ordinateurs avec des données fictives
    private static readonly List<Ordinateur> ordinateurs = new();

    // Méthode pour initialiser le parc d'ordinateurs avec des données fictives
    // public static void InitialiserParc()
    // {
    //     Ordinateur ordinateur1 = new Ordinateur(1, "AnaheimElectronic", "Omen", "16 Go", "Intel i5", EtatOrdinateur.Allume);
    //     Ordinateur ordinateur2 = new Ordinateur(2, "Minovki", "MSI", "32 Go", "AMD Ryzen 7", EtatOrdinateur.Eteint);
    //     Ordinateur ordinateur3 = new Ordinateur(3, "Gigi", "Dell", "32 Go", "AMD Ryzen 7", EtatOrdinateur.Eteint);
    //     Ordinateur ordinateur4 = new Ordinateur(4, "Hathaway", "Alienware", "64 Go", "AMD Ryzen 7", EtatOrdinateur.Eteint);
    //     Ordinateur ordinateur5 = new Ordinateur(5, "LaneAim", "ROG", "8 Go", "AMD Ryzen 7", EtatOrdinateur.Eteint);
    //     Add(ordinateur1);
    //     Add(ordinateur2);
    //     Add(ordinateur3);
    //     Add(ordinateur4);
    //     Add(ordinateur5);
    // }

    // Méthodes pour ajouter un ordinateur à la liste
    public static bool Add(Ordinateur ordinateur)
    {
        ordinateurs.Add(ordinateur);
        return true;
    }

    // Méthode pour supprimer un ordinateur de la liste par son ID
    public static bool Remove(int id)
    {
        Ordinateur? ordinateur = ordinateurs.FirstOrDefault(o => o.Id == id);
        if (ordinateur != null)
        {
            ordinateurs.Remove(ordinateur);
            return true;
        }

        return false;
    }

    // Méthode pour mettre à jour un ordinateur dans la liste par son ID
    public static bool Update(int id, string nom, Caracteristiques caracteristiques, EtatOrdinateur etat)
    {
        Ordinateur? ordinateurToUpdate = ordinateurs.FirstOrDefault(o => o.Id == id);
        if (ordinateurToUpdate != null)
        {
            ordinateurToUpdate.Nom = nom;
            ordinateurToUpdate.Caracteristiques = caracteristiques;
            ordinateurToUpdate.Etat = etat;
            return true;
        }

        return false;
    }

    // Méthode pour obtenir tous les ordinateurs
    public static IReadOnlyList<Ordinateur> GetAll()
    {
        return ordinateurs.AsReadOnly();
    }

    // Méthode pour obtenir un ordinateur par son ID
    public static IReadOnlyList<Ordinateur> GetById(int id)
    {
        return ordinateurs.Where(o => o.Id == id).ToList().AsReadOnly();
    }

    // Méthode pour obtenir les ordinateurs correspondant à une marque.
    public static IReadOnlyList<Ordinateur> GetByMarque(string marque)
    {
        // Crée une liste avec les ordinateurs dont la marque correspond à la recherche.
        List<Ordinateur> ordinateursParMarque = ordinateurs
            .Where(ordinateur => string.Equals(
                ordinateur.Marque,
                marque,
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Retourne la liste en lecture seule, comme dans GetAll().
        return ordinateursParMarque.AsReadOnly();
    }

}