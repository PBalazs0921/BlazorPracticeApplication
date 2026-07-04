using System.Net.Http.Headers;
using BlazorApp1.Endpoint.Settings;
using Microsoft.Extensions.Options;

namespace BlazorApp1.Endpoint.DelegatingHandlers;

public class ResendAuthHandler : DelegatingHandler
{
    private readonly ResendSettings _settings;

    public ResendAuthHandler(IOptions<ResendSettings> settings) => _settings = settings.Value;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        return base.SendAsync(request, ct);
    }
}