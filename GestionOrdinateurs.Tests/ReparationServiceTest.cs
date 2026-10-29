namespace GestionOrdinateurs.Tests;

using GestionOrdinateurs.API;
using GestionOrdinateurs.API.contexts;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

public class ReparationServiceTest
{
    [Fact]
    public async Task CreerReparation_EstEnregistreeDansLeContexte()
    {
        DbContextOptions<ParcInformatiqueContext> options =
            new DbContextOptionsBuilder<ParcInformatiqueContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        await using ParcInformatiqueContext context = new(options);
        Ordinateur ordinateur = new(
            1,
            "Dell XPS",
            "Dell",
            EtatOrdinateur.EnMaintenance,
            caracteristiques: new Caracteristiques(
                16,
                512,
                TypeOrdinateur.Portable,
                "Intel i5"));

        context.Ordinateurs.Add(ordinateur);
        await context.SaveChangesAsync();

        Reparation reparation = new(
            1,
            ordinateur.Id,
            ordinateur,
            "Réparation matérielle",
            DateTime.UtcNow,
            "Terminée",
            "Alice");

        context.Reparations.Add(reparation);
        await context.SaveChangesAsync();

        Reparation? resultat = await context.Reparations
            .SingleOrDefaultAsync(reparation => reparation.Id == 1);

        Assert.NotNull(resultat);
        Assert.Equal("Alice", resultat.Technicien);
        Assert.Equal(ordinateur.Id, resultat.OrdinateurId);
    }
}
