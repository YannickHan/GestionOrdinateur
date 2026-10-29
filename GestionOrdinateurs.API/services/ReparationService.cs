namespace GestionOrdinateurs.API;

using GestionOrdinateurs.API.contexts;
using GestionOrdinateurs.API.services.interfaces;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

public sealed class ReparationService : IReparationService
{
    private readonly ParcInformatiqueContext _context;
    private readonly SemaphoreSlim _contextLock = new(1, 1);
    private readonly JournalService _journalService;

    // Reçoit le contexte EF Core utilisé pour accéder aux réparations.
    public ReparationService(ParcInformatiqueContext context, JournalService? journalService = null)
    {
        _context = context;
        _journalService = journalService ?? new JournalService();
    }

    // Crée une réparation, l'enregistre, attend deux secondes, puis la marque comme terminée.
    // Écrit ensuite l'intervention terminée dans le journal des réparations.
    public async Task SimulerReparationAsync(Ordinateur ordinateur, string technicien = "Technicien par défaut")
    {
        Reparation reparation = new(
            0,
            ordinateur.Id,
            ordinateur,
            "Réparation matérielle",
            DateTime.UtcNow,
            "En cours",
            technicien);

        await _contextLock.WaitAsync();
        try
        {
            await _context.Reparations.AddAsync(reparation);
            await _context.SaveChangesAsync();
        }
        finally
        {
            _contextLock.Release();
        }

        await Task.Delay(2000);

        await _contextLock.WaitAsync();
        try
        {
            reparation.Statut = "Terminée";
            await _context.SaveChangesAsync();
        }
        finally
        {
            _contextLock.Release();
        }

        _journalService.EcrireReparation($"{ordinateur.Nom} réparé");
    }

    // Récupère toutes les réparations avec l'ordinateur associé.
    public async Task<IReadOnlyList<Reparation>> GetAllAsync()
    {
        return await _context.Reparations
            .Include(reparation => reparation.UnOrdinateur)
            .ToListAsync();
    }

    // Construit l'historique simplifié des réparations avec les informations utiles à l'API.
    public async Task<IReadOnlyList<HistoriqueReparationRecord>> GetHistoriqueAsync()
    {
        return await _context.Reparations
            .AsNoTracking()
            .Select(reparation => new HistoriqueReparationRecord(
                reparation.OrdinateurId,
                reparation.UnOrdinateur.Nom,
                reparation.Technicien,
                reparation.TypeReparation,
                reparation.DateDebut))
            .ToListAsync();
    }
}
