using BlazorApp1.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MySql;
using Xunit;

namespace BlazorApp1.IntegrationTests;

public class IntegrationTestWebAppFactory<TProgram> 
    : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
{
    private readonly MySqlContainer _container = new MySqlBuilder()
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
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var connectionString = _container.GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Container must be started before configuration.");
            }

            Console.WriteLine($"Using connection string: {connectionString}");

            var inMemorySettings = new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            };

            configBuilder.AddInMemoryCollection(inMemorySettings);
        });
        builder.ConfigureServices(services =>
        {
            var connectionString = _container.GetConnectionString();
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if ( descriptor is not null)
            {
                services.Remove(descriptor);
            }
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
        });



    }

}