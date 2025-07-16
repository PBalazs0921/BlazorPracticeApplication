using BlazorApp1;
using BlazorApp1.Components;
using BlazorApp1.Data;
using BlazorApp1.Entities;
using BlazorApp1.Logic;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddTransient<DtoProvider>();

builder.Services.AddScoped<Repository<User>>();
builder.Services.AddScoped<UserLogic>();

builder.Services.AddScoped<Repository<Item>>();
builder.Services.AddScoped<ItemLogic>();

builder.Services.AddScoped<Repository<Category>>();
builder.Services.AddScoped<CategoryLogic>();

builder.Services.AddScoped<Repository<Booking>>();
builder.Services.AddScoped<BookingLogic>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}




app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
//app.MapBlazorHub(); // Required for event handling
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();