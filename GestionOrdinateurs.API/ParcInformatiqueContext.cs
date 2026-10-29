using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API.Data;

public class ParcInformatiqueContext : DbContext
{
    public ParcInformatiqueContext(DbContextOptions<ParcInformatiqueContext> options)
        : base(options)
    {
    }

    public DbSet<Ordinateur> Ordinateurs { get; set; }
    public DbSet<Reparation> Reparations { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=parcinfo.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ordinateur>()
            .OwnsOne(o => o.Caracteristiques);

        modelBuilder.Entity<Ordinateur>()
            .HasMany(o => o.Reparations)
            .WithOne(r => r.UnOrdinateur)
            .HasForeignKey(r => r.OrdinateurId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ordinateur>().HasData(
            new{ Id = 1, Nom = "PC1", Marque = "HP", Processeur = "AMD Ryzen 5 5600H", Stock = 5, Prix = 800.0m, Vendu = false, etat = Ordinateur.EtatOrdinateur.EnService },
            new{ Id = 2, Nom = "PC2", Marque = "Dell", Processeur = "AMD Ryzen 7", Stock = 5, Prix = 800.0m, Vendu = false, etat = Ordinateur.EtatOrdinateur.EnService},
            new{ Id = 3, Nom = "PC3", Marque = "Asus", Processeur = "Intel 5", Stock = 5, Prix = 800.0m, Vendu = false, etat = Ordinateur.EtatOrdinateur.EnService},
            new{ Id = 4, Nom = "PC4", Marque = "MSI", Processeur = "Intel 7", Stock = 5, Prix = 800.0m, Vendu = false, etat = Ordinateur.EtatOrdinateur.EnService}
        );

        modelBuilder.Entity<Ordinateur>().OwnsOne(o => o.Caracteristiques).HasData(
        new { OrdinateurId = 1, RamGo = 8, StockageGo = 512, TypeOrdi = TypeOrdinateur.Fixe },
        new { OrdinateurId = 2, RamGo = 16, StockageGo = 512, TypeOrdi = TypeOrdinateur.Portable },
        new { OrdinateurId = 3, RamGo = 32, StockageGo = 1024, TypeOrdi = TypeOrdinateur.Fixe },
        new { OrdinateurId = 4, RamGo = 64, StockageGo = 2048, TypeOrdi = TypeOrdinateur.Portable }
        );

        modelBuilder.Entity<User>()
        .HasIndex(u => u.Login)
        .IsUnique();
    }
}