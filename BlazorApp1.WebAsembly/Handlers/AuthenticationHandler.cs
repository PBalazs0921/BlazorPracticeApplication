using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;

namespace BlazorApp1.WebAsembly.Handlers;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly IConfiguration _configuration;

    public AuthenticationHandler(IAccessTokenProvider tokenProvider, IConfiguration configuration)
    {
        _tokenProvider = tokenProvider;
        _configuration = configuration;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var isToServer = request.RequestUri?.AbsoluteUri.StartsWith(_configuration["ApiSettings:BaseUrl"] ?? "") ?? false;

        if (isToServer)
        {
            var tokenResult = await _tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out var token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
