using {{ProjectName}}.Framework;

namespace {{ProjectName}}.HybridApp.Services;

public class TokenHandler : DelegatingHandler
{
    private static string? _token;
    public TokenHandler(ILocalStorageService localStorageService)
    {
        LocalStorageService = localStorageService;
    }

    public ILocalStorageService LocalStorageService { get; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await base.SendAsync(request, cancellationToken);
    }
}
