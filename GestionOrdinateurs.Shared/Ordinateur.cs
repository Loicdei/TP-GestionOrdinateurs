using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestionOrdinateurs.Shared;

[Table("Ordinateur")]
public class Ordinateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Marque { get; set; } = string.Empty;
    public string Processeur { get; set; } = string.Empty;
    public bool Vendu { get; set; } = false;
    
    [DefaultValue(0)]
    public int Stock { get; set; } = 5;

    [DefaultValue(0)]
    public decimal Prix { get; set; } = 800.0m;

    public enum EtatOrdinateur
    {
        EnService,
        EnPanne,
        EnMaintenance,
        HorsService,
        Lent,
        Reparable
    }

    public EtatOrdinateur etat {get; set; } = EtatOrdinateur.EnService;

    public Caracteristiques Caracteristiques { get; set; } = new();

    public List<Reparation> Reparations { get; set; } = new();

    [JsonConstructor]
    public Ordinateur() { }

    protected Ordinateur(int id, string nom, string marque, string processeur, int stock = 5, decimal prix = 800.0m)
    {
        Id = id;
        Nom = nom;
        Marque = marque;
        Processeur = processeur;
        Stock = stock;
        Prix = prix;
    }

    // Permet de changer d'état
    public void ChangerEtat(EtatOrdinateur nouvelEtat)
    {
        etat = nouvelEtat;
    }
}