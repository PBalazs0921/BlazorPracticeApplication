
using BlazorApp1.Data;
using BlazorApp1.Data.Helper;
using BlazorApp1.Entities.Entity;
using Microsoft.AspNetCore.Identity;

namespace BlazorApp1.Endpoint;

public static class SeedData
{
    public static async Task InitializeAsync(
        ApplicationDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Ensure database exists
        context.Database.EnsureCreated();

        // === SEED CATEGORIES ===
        if (!context.Categories.Any())
        {
            var categories = new[]
            {
                new Category { Name = "Electronics" },
                new Category { Name = "Books" },
                new Category { Name = "Clothing" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            var items = new[]
            {
                new Item { Name = "Laptop", CategoryId = categories[0].Id },
                new Item { Name = "Smartphone", CategoryId = categories[0].Id },
                new Item { Name = "Novel", CategoryId = categories[1].Id },
                new Item { Name = "T-Shirt", CategoryId = categories[2].Id }
            };

            context.Items.AddRange(items);
            await context.SaveChangesAsync();
        }

        // === SEED ADMIN ROLE ===
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // === SEED ADMIN USER ===
        var adminEmail = "admin@system.local";
        var adminPassword = "Admin@123";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FamilyName = "System",
                GivenName = "Administrator",
                RefreshToken = "",
                RefreshTokenExpiryTime =  new DateTime(2030, 8, 25, 14, 30, 0) // YYYY, MM, DD, HH, MM, SS
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                throw new Exception($"Failed to create default admin: {string.Join("; ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}

