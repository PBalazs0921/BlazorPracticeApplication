using BlazorApp1.Data;
using BlazorApp1.Endpoint;
using BlazorApp1.Endpoint.Minimal.Endpoints;
using BlazorApp1.Logic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Use InMemory database only in development
        options.UseInMemoryDatabase("DevInMemoryDb");
        options.EnableSensitiveDataLogging();
    }
    else
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        options.UseMySQL(connectionString);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//LOAD SEED DATA
if (app.Environment.IsDevelopment())
{
    // Create the database schema automatically only in dev
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    SeedData.Initialize(db);
    
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/api/categories", CategoryEndpoints.GetAll);

app.MapGet("/api/categories/{id:int}", CategoryEndpoints.GetById);

app.Run();
