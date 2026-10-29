using GestionOrdinateurs.API.Data;
using GestionOrdinateurs.API.Events;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Services;

public class OrdinateurService : IOrdinateurService
{

    private readonly ParcInformatiqueContext _context;

    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

    public OrdinateurService(ParcInformatiqueContext context, JournalService journalService)
    {
        _context = context;

        OrdinateurAjoute += (sender, e) => 
            journalService.LogActionOrdinateur("Ajout", e.Ordinateur.Nom);

        OrdinateurMisAJour += (sender, e) => 
            journalService.LogActionOrdinateur("Modification", e.Ordinateur.Nom);

        OrdinateurSupprime += (sender, e) => 
            journalService.LogActionOrdinateur("Suppression", $"Ordinateur ID {e.OrdinateurId}");
    }
    
    // Récupérer tous les ordinateurs
    public async Task<List<Ordinateur>> GetAllAsync()
    {
        return await _context.Ordinateurs
            .Include(o => o.Reparations)
            .ToListAsync();
    }

    // Récupérer l'ordinateur qui contient l'ID renseigné
    public async Task<Ordinateur?> GetByIdAsync(int id)
    {
        return await _context.Ordinateurs
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    // Ajouter un ordinateur
    public async Task AddAsync(Ordinateur ordinateur)
    {
        _context.Ordinateurs.Add(ordinateur);
        await _context.SaveChangesAsync();

        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs(ordinateur));
    }

    // Modifier l'ordinateur qui contient l'ID renseigné
    public async Task UpdateAsync(int id, Ordinateur ordinateur)
    {
        var existant = await _context.Ordinateurs.FirstOrDefaultAsync(o => o.Id == id);
        if (existant is not null)
        {
            existant.Nom = ordinateur.Nom;
            existant.Marque = ordinateur.Marque;
            existant.Processeur = ordinateur.Processeur;
            existant.Stock = ordinateur.Stock;
            existant.Prix = ordinateur.Prix;

            if (ordinateur.Caracteristiques is not null)
            {
                existant.Caracteristiques = ordinateur.Caracteristiques;
            }

            await _context.SaveChangesAsync();

            OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs(existant));
        }
    }

    // Supprimer l'ordinateur qui contient l'ID renseigné
    public async Task DeleteAsync(int id)
    {
        var existant = await GetByIdAsync(id);
        if (existant is not null)
        {
            _context.Ordinateurs.Remove(existant);
            await _context.SaveChangesAsync();

            OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs(id));
        }
    }

    // Récupérer l'ordinateur par la marque
    public async Task<List<Ordinateur>> GetByMarqueAsync(string marque)
    {
        return await _context.Ordinateurs
            .Where(o => o.Marque.ToLower() == marque.ToLower())
            .ToListAsync();
    }

    // Récupérer les ordinateurs disponibles
    public async Task<List<Ordinateur>> GetDisponiblesAsync()
    {
        return await _context.Ordinateurs
            .Where(o => o.Stock > 0)
            .ToListAsync();
    }

    // Récupérer les ordinateurs du plus cher au moins cher
    public async Task<List<Ordinateur>> GetTriPrixDecroissantAsync()
    {
        return await _context.Ordinateurs
            .OrderByDescending(o => o.Prix)
            .ToListAsync();
    }

    //Permet de diagnostiquer un ordinateur
    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            {
                etat: Ordinateur.EtatOrdinateur.HorsService
            }
                => "Remplacement",

            {
                etat: Ordinateur.EtatOrdinateur.Lent,
                Caracteristiques:
                {
                    RamGo: < 8
                }
            }
                => "Ajouter RAM",

            _
                => "Aucune action"
        };
    }

    //Permet de calculer les statistiques
    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques()
    {
        var totalOrdinateurs = _context.Ordinateurs.Count();
        var ordiDisponibles = _context.Ordinateurs.Count(o => o.Stock > 0);
        var valeurStock = _context.Ordinateurs.Sum(o => o.Prix * o.Stock);
        var nbReparations = _context.Reparations.Count();

        return (totalOrdinateurs, ordiDisponibles, valeurStock, nbReparations);
    }
}