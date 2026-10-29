namespace GestionOrdinateurs.API.Services;

public class JournalService
{
    private readonly string _dossierReparations = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "Reparations");
    private readonly string _dossierOrdinateurs = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "Ordinateurs");

    public JournalService()
    {
        if (!Directory.Exists(_dossierReparations))
        {
            Directory.CreateDirectory(_dossierReparations);
        }

        if (!Directory.Exists(_dossierOrdinateurs))
        {
            Directory.CreateDirectory(_dossierOrdinateurs);
        }
    }

    public void LogReparation(string nomOrdinateur)
    {
        var cheminFichier = Path.Combine(_dossierReparations, $"reparations_{DateTime.Now:yyyy-MM-dd}.log");

        using var writer = new StreamWriter(cheminFichier, append: true);
        writer.WriteLine($"{DateTime.Now:dd/MM/yyyy} - {nomOrdinateur} réparé");
    }

    public void LogActionOrdinateur(string action, string details)
    {
        var cheminFichier = Path.Combine(_dossierOrdinateurs, $"ordinateurs_{DateTime.Now:yyyy-MM-dd}.log");

        using var writer = new StreamWriter(cheminFichier, append: true);
        writer.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - Action: {action} | Détails: {details}");
    }
}