using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.API.Data;
using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.Tests;

public class ReparationServiceTests
{
    private ParcInformatiqueContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ParcInformatiqueContext(options);
    }

    [Fact]
    public async Task SimulerReparationAsync_DevraitAjouterReparationEnBase()
    {
        using var context = GetInMemoryDbContext();
        var journalService = new JournalService();
        var service = new ReparationService(context, journalService);

        var ordinateur = new Ordinateur
        {
            Id = 1,
            Nom = "PC-Test",
            Marque = "Dell",
            Processeur = "Intel i7",
            Stock = 5,
            Prix = 1000m
        };
        context.Ordinateurs.Add(ordinateur);
        await context.SaveChangesAsync();

        await service.SimulerReparationAsync(ordinateur);

        var reparations = await context.Reparations.ToListAsync();
        Assert.Single(reparations);
        Assert.Equal(ordinateur.Id, reparations[0].OrdinateurId);
        Assert.Equal("Terminée", reparations[0].Statut);
        Assert.Equal("Alice", reparations[0].Technicien);
    }
}