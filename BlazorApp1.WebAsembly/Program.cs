using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp1.WebAsembly;
using BlazorApp1.WebAsembly.Handlers;
using BlazorApp1.WebAsembly.Services;
using Blazored.SessionStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after"); 

// Register the authentication service
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
// Register the category repository service
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//Blazorise configuration
builder.Services.AddBlazoredSessionStorageAsSingleton();


builder.Services.AddTransient<AuthenticationHandler>();

builder.Services.AddHttpClient("ServerApi")
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? ""))
    .AddHttpMessageHandler<AuthenticationHandler>();

await builder.Build().RunAsync();
