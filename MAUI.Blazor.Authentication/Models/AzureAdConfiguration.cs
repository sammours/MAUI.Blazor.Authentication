namespace MAUI.Blazor.Authentication.Models
{
    public class AzureAdConfiguration
    {
        private string authority = "https://login.microsoftonline.com/";

        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string ReturnedUrl { get; set; }

        public string[] Scopes { get; set; }

        public string Authority => $"{authority}{this.TenantId}";
    }
}
