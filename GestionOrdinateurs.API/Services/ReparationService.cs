using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.API.Data;
using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API.Services;

public record HistoriqueReparationRecord(
    int OrdinateurId,
    string NomOrdinateur,
    string Technicien,
    string TypeReparation,
    DateTime DateIntervention
);

public class ReparationService
{
    private readonly ParcInformatiqueContext _context;
    private readonly JournalService _journalService;

    public ReparationService(ParcInformatiqueContext context, JournalService journalService)
    {
        _context = context;
        _journalService = journalService;
    }

    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        var test = new Reparation
        {
            OrdinateurId = ordinateur.Id,
            TypeReparation = "Réparation",
            DateDebut = DateTime.Now,
            Statut = "En cours",
            Technicien = "Alice"
        };

        _context.Reparations.Add(test);
        await _context.SaveChangesAsync();

        await Task.Delay(2000);

        test.Statut = "Terminée";
        await _context.SaveChangesAsync();

        Console.WriteLine($"{ordinateur.Nom} réparé");
        _journalService.LogReparation(ordinateur.Nom);
    }

    public async Task<List<Reparation>> GetAllAsync()
    {
        return await _context.Reparations
            .Include(r => r.UnOrdinateur)
            .ToListAsync();
    }

    public async Task<List<HistoriqueReparationRecord>> GetHistoriqueAsync()
    {
        return await _context.Reparations
            .Include(r => r.UnOrdinateur)
            .Select(r => new HistoriqueReparationRecord(
                r.OrdinateurId,
                r.UnOrdinateur != null ? r.UnOrdinateur.Nom : "Inconnu",
                r.Technicien,
                r.TypeReparation,
                r.DateDebut
            ))
            .ToListAsync();
    }
}