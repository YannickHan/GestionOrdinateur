using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.contexts;
public class ParcInformatiqueContext : DbContext
{
    public ParcInformatiqueContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Reparation> Reparations { get; set; }

    public DbSet<Ordinateur> Ordinateurs { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(user => user.Login)
            .IsUnique();

        modelBuilder.Entity<Ordinateur>()
            .HasKey(ordinateur => ordinateur.Id);
        
        modelBuilder.Entity<Ordinateur>()
            .OwnsOne(o => o.Caracteristiques);

        modelBuilder.Entity<Ordinateur>()
            .HasMany(o => o.Reparations)
            .WithOne(r => r.UnOrdinateur);

        modelBuilder.Entity<Reparation>()
            .HasKey(reparation => reparation.Id);

        modelBuilder.Entity<Reparation>()
            .HasOne(reparation => reparation.UnOrdinateur)
            .WithMany()
            .HasForeignKey(reparation => reparation.OrdinateurId)
            .HasPrincipalKey(ordinateur => ordinateur.Id);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=parcinfo.db");
        }
    }
}