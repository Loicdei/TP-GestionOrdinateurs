using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.API.Data;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace GestionOrdinateurs.Tests;

public class OrdinateurServiceTest
{
    private ParcInformatiqueContext CreerContexte()
    {
        var options =
            new DbContextOptionsBuilder<ParcInformatiqueContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new ParcInformatiqueContext(options);
    }

    [Fact]
    public async Task AddAsync_DevraitAjouterOrdinateurDansLaBase()
    {
        using var context = CreerContexte();
        var journalService = new JournalService();
        var service = new OrdinateurService(context, journalService);

        var nouveauPc = new Ordinateur
        {
            Id = 99, Nom = "PC Test", Marque = "Lenovo", Processeur = "Intel i5", Stock = 10, Prix = 950.0m,
            Caracteristiques = new Caracteristiques(8, 512, TypeOrdinateur.Portable)
        };

        await service.AddAsync(nouveauPc);

        var liste = await service.GetAllAsync();
        liste.Count.Should().Be(1);

        var pcEnBdd = await service.GetByIdAsync(99);
        pcEnBdd.Should().NotBeNull();
        pcEnBdd!.Stock.Should().Be(10);
        pcEnBdd.Prix.Should().Be(950.0m);
    }

    [Fact]
    public async Task DeleteAsync_AvecIdExistant_DevraitSupprimerOrdinateur()
    {
        using var context = CreerContexte();
        var journalService = new JournalService();
        var service = new OrdinateurService(context, journalService);

        var pcExistant = new Ordinateur
        {
            Id = 100, Nom = "PC A Supprimer", Marque = "Dell", Processeur = "i3", Stock = 1, Prix = 400.0m,
            Caracteristiques = new Caracteristiques(8, 256, TypeOrdinateur.Fixe)
        };
        await service.AddAsync(pcExistant);

        await service.DeleteAsync(100);

        var pcSupprime = await service.GetByIdAsync(100);
        pcSupprime.Should().BeNull();
    }

    [Fact]
    public async Task GetTriPrixDecroissantAsync_DevraitRenvoyerOrdinateursTriesPrix()
    {
        using var context = CreerContexte();
        var journalService = new JournalService();
        var service = new OrdinateurService(context, journalService);

        var pcMoinsCher = new Ordinateur { Id = 1, Nom = "PC Pas Cher", Marque = "HP", Prix = 500.0m };
        var pcPlusCher = new Ordinateur { Id = 2, Nom = "PC Gamer", Marque = "Asus", Prix = 1500.0m };

        await service.AddAsync(pcMoinsCher);
        await service.AddAsync(pcPlusCher);

        var resultat = await service.GetTriPrixDecroissantAsync();

        resultat.Should().HaveCount(2);
        resultat.First().Prix.Should().Be(1500.0m);
        resultat.Last().Prix.Should().Be(500.0m);
    }

    [Fact]
    public async Task GetDisponiblesAsync_NeDevraitRenvoyerQueOrdinateursAvecStockSuperieurZero()
    {
        using var context = CreerContexte();
        var journalService = new JournalService();
        var service = new OrdinateurService(context, journalService);

        var pcEnStock = new Ordinateur { Id = 1, Nom = "PC Dispo", Stock = 5 };
        var pcHorsStock = new Ordinateur { Id = 2, Nom = "PC Epuise", Stock = 0 };

        await service.AddAsync(pcEnStock);
        await service.AddAsync(pcHorsStock);

        var resultat = await service.GetDisponiblesAsync();

        resultat.Should().HaveCount(1);
        resultat.First().Id.Should().Be(1);
    }
}