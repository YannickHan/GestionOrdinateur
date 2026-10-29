using GestionOrdinateurs.Shared;
namespace GestionOrdinateurs.API.services.interfaces;
public interface IReparationService
{
    // Lance la simulation de réparation d'un ordinateur.
    Task SimulerReparationAsync(Ordinateur ordinateur, string technicien = "Technicien par défaut");

    // Récupère la liste des réparations enregistrées.
    Task<IReadOnlyList<Reparation>> GetAllAsync();
    Task<IReadOnlyList<HistoriqueReparationRecord>> GetHistoriqueAsync();
}