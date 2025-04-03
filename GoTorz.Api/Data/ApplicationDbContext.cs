using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GoTorz.Shared.Models;

namespace GoTorz.Api.Data;

/// <summary>
/// Connects application to the database using Entity Framework Core
/// Inherits from IdentityDbContext to include Identity (Users, Roles, Claims, Tokens, etc.).
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Flight> Flights { get; set; }
    public DbSet<ReturnFlight> ReturnFlights { get; set; }
    public DbSet<OutboundFlight> OutboundFlights { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<TravelPackage> TravelPackages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure inheritance for Flight (TPH: Table-per-Hierarchy)
        modelBuilder.Entity<Flight>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<OutboundFlight>("OutboundFlight")
            .HasValue<ReturnFlight>("ReturnFlight");

        // Configure TravelPackage foreign keys
        modelBuilder.Entity<TravelPackage>()
            .HasOne(tp => tp.Hotel)
            .WithMany()
            .HasForeignKey(tp => tp.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TravelPackage>()
            .HasOne(tp => tp.OutboundFlight)
            .WithMany()
            .HasForeignKey(tp => tp.OutboundFlightFlightId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TravelPackage>()
            .HasOne(tp => tp.ReturnFlight)
            .WithMany()
            .HasForeignKey(tp => tp.ReturnFlightFlightId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents multiple cascade paths
    }
}
