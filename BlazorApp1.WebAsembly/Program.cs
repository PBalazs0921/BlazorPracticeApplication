using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp1.WebAsembly;
using BlazorApp1.WebAsembly.Handlers;
using BlazorApp1.WebAsembly.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
    options.ProviderOptions.ClientId = builder.Configuration["Auth0:ClientId"]!;
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]!);
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddTransient<AuthenticationHandler>();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
    throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");

builder.Services.AddHttpClient("ServerApi")
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthenticationHandler>();

await builder.Build().RunAsync();
