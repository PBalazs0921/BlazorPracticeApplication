/*using BlazorApp1.Data;
using BlazorApp1.Entities.Entity;
using Testcontainers.MySql;
using Microsoft.EntityFrameworkCore;


namespace UnitTests;

public class TestContainer
{
    private MySqlContainer _container = null!;

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
        await using var db = CreateDbContext();
        // Your service test code here
    }
    
    [Test]    
    public async Task GetAllItems_ShouldReturnAllInsertedItems()
    {
        await using var ctx = CreateDbContext();
        var repo = new Repository<User>(ctx);

        await repo.CreateAsync(new User { Name = "A" });
        await repo.CreateAsync(new User { Name = "B" });

        var allItems = repo.GetAll().ToList();

        Assert.That(allItems.Count, Is.EqualTo(2));
    }

}*/