namespace MAUI.Blazor.Authentication.Clients
{
    using MAUI.Blazor.Authentication.Models;
    using Microsoft.Identity.Client;

    public class MachineAuthenticationClient : IMachineAuthenticationClient
    {
        private readonly IConfidentialClientApplication client;

        public MachineAuthenticationClient(AzureAdConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            this.client = ConfidentialClientApplicationBuilder.Create(configuration.ClientId)
                    .WithClientSecret(configuration.ClientSecret)
                    .WithAuthority(new Uri(configuration.Authority))
                    .Build();
        }

        public async Task<AuthenticationResult> AcquireTokenPrompetAsync(CancellationToken cancellationToken = default)
        {
            return await this.AcquireTokenPrompetAsync(new string[] { "https://graph.microsoft.com/.default" }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<AuthenticationResult> AcquireTokenPrompetAsync(string[] scopes, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await this.client.AcquireTokenForClient(scopes).ExecuteAsync(cancellationToken).ConfigureAwait(false);
                return result ?? default;
            }
            catch (MsalUiRequiredException)
            {
                // The application doesn't have sufficient permissions.
                // - Did you declare enough app permissions during app creation?
                // - Did the tenant admin grant permissions to the application?
                throw;
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be in the form "https://resourceurl/.default"
                // Mitigation: Change the scope to be as expected.
                throw;
            }
        }

        public CurrentUser ResolveToken(AuthenticationResult result)
        {
            throw new NotImplementedException();
        }
    }
}
