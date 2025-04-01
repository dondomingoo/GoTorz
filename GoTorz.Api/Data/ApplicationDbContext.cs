using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoTorz.Api.Data;

/// <summary>
/// Connects application to the database using Entity Framework Core
/// Inherits from IdentityDbContext to include Identity (Users, Roles, Claims, Tokens, etc.).
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


    // DbSet<TEntity> Entities { get; set; }   // Add more custom tables

}


