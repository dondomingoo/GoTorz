using Microsoft.AspNetCore.Identity;

namespace GoTorz.API.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Define the users to be seeded
            var users = new[]
            {
                new { Email = "1@1", Password = "1", Role = "Admin" },
                new { Email = "2@2", Password = "2", Role = "SalesRep" },
                new { Email = "3@3", Password = "3", Role = "User" },
            };

            // Ensure roles exist, create them if they don't
            var roles = new[] { "Admin", "SalesRep", "User" };
            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role) == false)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Loop through users and create them if they don't already exist
            foreach (var entry in users)
            {
                if (await userManager.FindByEmailAsync(entry.Email) is null)
                {
                    var user = new IdentityUser { UserName = entry.Email, Email = entry.Email };
                    var result = await userManager.CreateAsync(user, entry.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, entry.Role);
                    }
                }
            }
        }
    }
}

