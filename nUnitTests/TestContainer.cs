using BlazorApp1;
using BlazorApp1.Components;
using BlazorApp1.Entities;

using BlazorApp1.Data;
using NUnit.Framework;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Testcontainers.MySql;
using Microsoft.EntityFrameworkCore;


namespace nUnitTests;

public class TestContainer
{
    private MySqlContainer _container = default!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _container = new MySqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpw")
            .Build();

        await _container.StartAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _container.DisposeAsync();
    }

    private ApplicationDbContext CreateDbContext()
    {
        var connectionString = _container.GetConnectionString(); // e.g. "Server=...;Port=...;User=...;Password=...;Database=..."
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated(); // Or use Migrate()
        return context;
    }

    [Test]
    public async Task SampleTest()
    {
        using var db = CreateDbContext();
        // Your service test code here
    }
    
    [Test]    
    public async Task GetAllItems_ShouldReturnAllInsertedItems()
    {
        using var ctx = CreateDbContext();
        var repo = new Repository<User>(ctx);

        repo.Create(new User { Name = "A" });
        repo.Create(new User { Name = "B" });

        var allItems = repo.GetAll().ToList();

        Assert.AreEqual(2, allItems.Count);
    }

}