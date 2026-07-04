using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Trace;
using BlazorApp1.Data;
using BlazorApp1.Endpoint.DelegatingHandlers;
using BlazorApp1.Endpoint.Services;
using BlazorApp1.Endpoint.Settings;
using BlazorApp1.Logic;
using BlazorApp1.Logic.Dto;
using BlazorApp1.Logic.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<ResendSettings>()
    .BindConfiguration(ResendSettings.ConfigurationSection)
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddTransient<ResendAuthHandler>();

builder.Services.AddHttpClient<ResendEmailClient>(client =>
    {
        client.BaseAddress = new Uri("https://api.resend.com/");
    })
    .AddHttpMessageHandler<ResendAuthHandler>();

//SWAGGER, TOKEN
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "MovieClub API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    var requirement = new OpenApiSecurityRequirement();
    requirement.Add(new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    }, new string[] { });
    option.AddSecurityRequirement(requirement);
});





//DB context
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

        options.UseNpgsql(connectionString);
    }
});


builder.Services.AddTransient(typeof(Repository<>));

builder.Services.AddTransient<DtoProvider>();


builder.Services.AddTransient<ICategoryLogic, CategoryLogic>();
builder.Services.AddTransient<IItemLogic, ItemLogic>();

//AUTH
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };
    });

//OPENTELEMETRY
builder.Services.AddOpenTelemetry()
    .UseAzureMonitor(o => o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"])
    .WithTracing(t => t.AddEntityFrameworkCoreInstrumentation());

//CORS for webassembly
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }