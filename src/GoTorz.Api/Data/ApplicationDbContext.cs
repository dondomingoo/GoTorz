using GoTorz.Shared.Models;
using Microsoft.AspNetCore.Identity;
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
    public DbSet<Flight> Flights => Set<Flight>(); // VIGTIGT for EF TPT!

    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Traveller> Travellers => Set<Traveller>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //  Primærnøgle og tabelnavn for base class Flight
        modelBuilder.Entity<Flight>(f =>
        {
            f.HasKey(f => f.FlightId); // PRIMÆRNØGLE HER!
            f.ToTable("Flights");
        });

        //  TPT-konfigurationer for subklasser
        modelBuilder.Entity<OutboundFlight>().ToTable("OutboundFlights");
        modelBuilder.Entity<ReturnFlight>().ToTable("ReturnFlights");

        // TravelPackage-konfigurationer
        modelBuilder.Entity<TravelPackage>(tp =>
        {
            tp.HasKey(t => t.TravelPackageId);

            tp.HasOne(t => t.Hotel)
              .WithMany()
              .HasForeignKey(t => t.HotelId)
              .OnDelete(DeleteBehavior.Restrict);

            tp.HasOne(t => t.OutboundFlight)
              .WithMany()
              .HasForeignKey(t => t.OutboundFlightId)
              .OnDelete(DeleteBehavior.Restrict);

            tp.HasOne(t => t.ReturnFlight)
              .WithMany()
              .HasForeignKey(t => t.ReturnFlightId)
              .OnDelete(DeleteBehavior.Restrict);

            tp.ToTable("TravelPackages");
        });
        
        // Booking-configurations
        modelBuilder.Entity<Booking>(b =>
        {
            b.HasMany(b => b.Travellers)
             .WithOne(t => t.Booking)
             .HasForeignKey(t => t.BookingId)
             .OnDelete(DeleteBehavior.Cascade);

            
            b.HasOne<IdentityUser>()
             .WithMany()
             .HasForeignKey(b => b.UserId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        base.OnModelCreating(modelBuilder);
    }
}



