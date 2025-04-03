using GoTorz.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoTorz.Api.Data;

/// <summary>
/// Connects application to the database using Entity Framework Core
/// Inherits from IdentityDbContext to include Identity (Users, Roles, Claims, Tokens, etc.).
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<TravelPackage> TravelPackages => Set<TravelPackage>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<OutboundFlight> OutboundFlights => Set<OutboundFlight>();
    public DbSet<ReturnFlight> ReturnFlights => Set<ReturnFlight>();
    public DbSet<Flight> Flights => Set<Flight>(); // 🧠 VIGTIGT for EF TPT!

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 🧠 Registrér base class og nøgle
        modelBuilder.Entity<Flight>(f =>
        {
            f.HasKey(f => f.FlightId);
            f.ToTable("Flights"); // base tabel
        });

        modelBuilder.Entity<OutboundFlight>(f =>
        {
            f.HasBaseType<Flight>();
            f.ToTable("OutboundFlights"); // derived tabel
        });

        modelBuilder.Entity<ReturnFlight>(f =>
        {
            f.HasBaseType<Flight>();
            f.ToTable("ReturnFlights"); // derived tabel
        });

        modelBuilder.Entity<TravelPackage>(tp =>
        {
            tp.HasKey(t => t.TravelPackageId);

            tp.HasOne(t => t.Hotel).WithMany().OnDelete(DeleteBehavior.Cascade);
            tp.HasOne(t => t.OutboundFlight).WithMany().OnDelete(DeleteBehavior.Cascade);
            tp.HasOne(t => t.ReturnFlight).WithMany().OnDelete(DeleteBehavior.Cascade);
        });
    }
}


