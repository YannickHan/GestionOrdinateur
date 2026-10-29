namespace GestionOrdinateurs.API;

public sealed class StockService
{
    // Objet utilisé pour protéger l'accès concurrent au stock.
    private readonly object _stockLock = new();

    // Stock SSD initialisé à 10 unités.
    public int StockSSD { get; private set; } = 10;

    // Simule la consommation d'un SSD.
    public void ConsommerSSD()
    {
        // Délai ajouté pour rendre visible le problème de concurrence.
        Task.Delay(10).Wait();

        // Une seule tâche à la fois peut modifier le stock.
        lock (_stockLock)
        {
            if (StockSSD > 0)
            {
                StockSSD--;
            }
        }
    }
}
