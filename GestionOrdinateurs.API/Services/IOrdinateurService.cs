using GestionOrdinateurs.API.Events;
using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API.Services;

public interface IOrdinateurService
{
    event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;
    
    // Récupère la liste complète des ordinateurs.
    Task<List<Ordinateur>> GetAllAsync();

    // Récupère un ordinateur par son identifiant.    
    Task<Ordinateur?> GetByIdAsync(int id);

    // Ajoute un nouvel ordinateur.    
    Task AddAsync(Ordinateur ordinateur);

    // Supprime un ordinateur à partir de son identifiant.    
    Task DeleteAsync(int id);

    // Met à jour les informations de l'ordinateur renseigné par l'Id.    
    Task UpdateAsync(int id, Ordinateur ordinateur);

    // Recherche les ordinateurs correspondant à une marque spécifique.
    Task<List<Ordinateur>> GetByMarqueAsync(string marque);

    // Recherche les ordinateurs disponibles
    Task<List<Ordinateur>> GetDisponiblesAsync();

    //Recherche les ordinateurs du plus cher au moins cher
    Task<List<Ordinateur>> GetTriPrixDecroissantAsync();

    //Permet de diagnostiquer un ordinateur
    string Diagnostiquer(Ordinateur ordinateur);

    //Permet de calculer les statistiques
    (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques();
}