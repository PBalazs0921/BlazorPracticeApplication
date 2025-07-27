using BlazorApp1.Data;
using BlazorApp1.Entities.Entity;

namespace BlazorApp1.Endpoint;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Check if there are any Categories already
        if (context.Categories.Any())
        {
            return;   // DB has been seeded
        }

        // Seed Categories
        var categories = new[]
        {
            new Category { Name = "Electronics" },
            new Category { Name = "Books" },
            new Category { Name = "Clothing" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        // Seed Items
        var items = new[]
        {
            new Item { Name = "Laptop", CategoryId = categories[0].Id },
            new Item { Name = "Smartphone", CategoryId = categories[0].Id },
            new Item { Name = "Novel", CategoryId = categories[1].Id },
            new Item { Name = "T-Shirt", CategoryId = categories[2].Id }
        };

        context.Items.AddRange(items);
        context.SaveChanges();
    }
}