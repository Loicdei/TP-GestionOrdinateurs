using System.Diagnostics;

namespace GestionOrdinateurs.Shared;

public static class ParcInitializer
{
    public static List<Ordinateur> ordinateurs = new List<Ordinateur> {
        
        new Ordinateur
        {
            Id = 1, Nom = "PC1", Marque = "HP", Processeur = "AMD Ryzen 5 5600H", Stock = 5, Prix = 800.0m,
            Caracteristiques = new Caracteristiques(8, 512, TypeOrdinateur.Fixe)
        },
        new Ordinateur
        {
            Id = 2, Nom = "PC2", Marque = "Dell", Processeur = "AMD Ryzen 7", Stock = 5, Prix = 800.0m,
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable)
        },
        new Ordinateur
        {
            Id = 3, Nom = "PC3", Marque = "Asus", Processeur = "Intel 5", Stock = 5, Prix = 800.0m,
            Caracteristiques = new Caracteristiques(32, 1024, TypeOrdinateur.Fixe)
        },
        new Ordinateur
        {
            Id = 4, Nom = "PC4", Marque = "MSI", Processeur = "Intel 7", Stock = 5, Prix = 800.0m,
            Caracteristiques = new Caracteristiques(64, 2048, TypeOrdinateur.Portable)
        },
        new Ordinateur
        {
            Id = 5, Nom = "PC5", Marque = "Legion", Processeur = "Intel 9", Stock = 5, Prix = 800.0m,
            Caracteristiques = new Caracteristiques(4, 256, TypeOrdinateur.Fixe)
        }
    };
}