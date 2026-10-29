namespace GestionOrdinateurs.API;

public sealed class JournalService
{
    private readonly string _reparationsLogPath;
    private readonly string _ordinateursLogPath;
    private readonly object _lock = new();

    public JournalService()
        : this(Directory.GetCurrentDirectory())
    {
    }

    public JournalService(IHostEnvironment environment)
        : this(environment.ContentRootPath)
    {
    }

    private JournalService(string contentRootPath)
    {
        string logsDirectory = Path.Combine(contentRootPath, "Logs");
        _reparationsLogPath = Path.Combine(logsDirectory, "Reparations", "reparations.log");
        _ordinateursLogPath = Path.Combine(logsDirectory, "Ordinateurs", "ordinateurs.log");
    }

    public void EcrireReparation(string message)
    {
        Ecrire(_reparationsLogPath, message);
    }

    public void EcrireOrdinateur(string message)
    {
        Ecrire(_ordinateursLogPath, message);
    }

    private void Ecrire(string path, string message)
    {
        string? directory = Path.GetDirectoryName(path);
        if (directory is null)
        {
            throw new InvalidOperationException("Le dossier de journalisation est introuvable.");
        }

        lock (_lock)
        {
            Directory.CreateDirectory(directory);
            using StreamWriter writer = new(path, append: true);
            writer.WriteLine($"{DateTime.Now:dd/MM/yyyy} - {message}");
        }
    }
}
