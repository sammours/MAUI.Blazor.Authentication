namespace MAUI.Blazor.Authentication.Clients
{
    using MAUI.Blazor.Authentication.Models;
    using Microsoft.Identity.Client;

    public interface IAuthenticationClient
    {
        Task<AuthenticationResult> AcquireTokenPrompetAsync(CancellationToken cancellationToken = default);

        Task<AuthenticationResult> AcquireTokenPrompetAsync(string[] scopes, CancellationToken cancellationToken = default);

        CurrentUser ResolveToken(AuthenticationResult result);
    }
}
