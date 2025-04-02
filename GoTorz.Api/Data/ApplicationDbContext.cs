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


    // DbSet<TEntity> Entities { get; set; }   // Add more custom tables
    public DbSet<Flight> Flights { get; set; }
    public DbSet<ReturnFlight> ReturnFlights { get; set; }
    public DbSet<OutboundFlight> OutboundFlights { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<TravelPackage> TravelPackages { get; set; }

}


