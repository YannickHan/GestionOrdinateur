namespace GestionOrdinateurs.Tests;

using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.contexts;
using Microsoft.EntityFrameworkCore;

public class OrdinateurServiceTest
{
    private ParcInformatiqueContext CreerContexte()
    {
        var options =
            new DbContextOptionsBuilder<ParcInformatiqueContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new ParcInformatiqueContext(options); // Crée un nouveau contexte de base de données en mémoire pour les tests.
    }
    
    // Vérifie qu'un ordinateur est bien ajouté dans le parc.
    [Fact]
    public void AjouterOrdinateur_DoItAjouterOrdinateur()
    {
        ParcInformatiqueContext context = CreerContexte();
        OrdinateurService service = new(context);
        const int id = 1001;
        bool resultat = service.Add(
            id,
            "Ordinateur test",
            "Dell",
            16,
            512,
            TypeOrdinateur.Portable,
            "Intel i5",
            EtatOrdinateur.EnService);

        IReadOnlyList<Ordinateur> ordinateurs = context.Ordinateurs
            .Where(o => o.Id == id)
            .ToList()
            .AsReadOnly();

        Assert.True(resultat);
        Assert.NotEmpty(ordinateurs);
        Assert.Equal(id, ordinateurs[0].Id);

        service.Remove(id);
    }

    // Vérifie qu'un ordinateur existant est bien supprimé du parc.
    [Fact]
    public void SupprimerOrdinateur_DoItSupprimerOrdinateurExistant()
    {
        ParcInformatiqueContext context = CreerContexte();
        OrdinateurService service = new(context);
        const int id = 1002;
        bool ajoutReussi = service.Add(
            id,
            "Ordinateur test",
            "Dell",
            16,
            512,
            TypeOrdinateur.Portable,
            "Intel i5",
            EtatOrdinateur.EnService);

        bool resultat = service.Remove(id);
        IReadOnlyList<Ordinateur> ordinateurs = context.Ordinateurs
            .Where(o => o.Id == id)
            .ToList()
            .AsReadOnly();

        Assert.True(ajoutReussi);
        Assert.True(resultat);
        Assert.Empty(ordinateurs);
    }

    // Vérifie qu'une suppression échoue si l'ordinateur n'existe pas.
    [Fact]
    public void SupprimerOrdinateur_RetourneFalseSiInexistant()
    {
        ParcInformatiqueContext context = CreerContexte();
        OrdinateurService service = new(context);
        const int id = 9999;

        bool resultat = service.Remove(id);

        Assert.False(resultat);
    }

    // Vérifie qu'un ordinateur peut être retrouvé grâce à sa marque.
    [Fact]
    public void RechercherOrdinateursParMarque_RetourneLesOrdinateursCorrespondants()
    {
        ParcInformatiqueContext context = CreerContexte();
        OrdinateurService service = new(context);
        const int id = 1004;
        const string marque = "Lenovo";
        bool ajoutReussi = service.Add(
            id,
            "Ordinateur test",
            marque,
            16,
            512,
            TypeOrdinateur.Portable,
            "Intel i5",
            EtatOrdinateur.EnService);

        IReadOnlyList<Ordinateur> ordinateurs = service.GetByMarque(marque);

        Assert.True(ajoutReussi);
        Assert.Contains(ordinateurs, ordinateur => ordinateur.Id == id);

        service.Remove(id);
    }

}
