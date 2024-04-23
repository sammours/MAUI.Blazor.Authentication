namespace MAUI.Blazor.Authentication.Clients
{
    using MAUI.Blazor.Authentication.Models;
    using Microsoft.Identity.Client;
    using System.IdentityModel.Tokens.Jwt;

    public class PublicAuthenticationClient : IPublicAuthenticationClient
    {
        private readonly IPublicClientApplication client;

        public PublicAuthenticationClient(AzureAdConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            this.client = PublicClientApplicationBuilder.Create(configuration.ClientId)
                                .WithAuthority(configuration.Authority)
                                .WithRedirectUri(configuration.ReturnedUrl)
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
                var result = await this.client
                            .AcquireTokenInteractive(scopes)
                            .WithPrompt(Microsoft.Identity.Client.Prompt.ForceLogin)
#if ANDROID
                            .WithParentActivityOrWindow(Platform.CurrentActivity)
#endif
                            .ExecuteAsync(cancellationToken).ConfigureAwait(false);
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
            var token = result?.IdToken; // you can also get AccessToken if you need it
            if (token != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var data = handler.ReadJwtToken(token);
                var claims = data.Claims.ToList();
                if (data != null)
                {
                    return new CurrentUser()
                    {
                        Name = data.Claims.FirstOrDefault(x => x.Type.Equals("name"))?.Value,
                        Email = data.Claims.FirstOrDefault(x => x.Type.Equals("preferred_username"))?.Value,
                    };
                }
            }

            return default;
        }
    }
}
