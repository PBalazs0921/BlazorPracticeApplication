using BlazorApp1.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace BlazorApp1.IntegrationTests;

public class IntegrationTestWebAppFactory<TProgram>
    : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpw")
        .Build();

    public HttpClient HttpClient { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            var connectionString = _container.GetConnectionString();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
        });
    }
}
