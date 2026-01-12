using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

namespace VisorDoc.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private IConfidentialClientApplication _app;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            var azureAdConfig = _configuration.GetSection("AzureAd");

            var clientId = azureAdConfig["ClientId"];
            var clientSecret = azureAdConfig["ClientSecret"];
            var tenantId = azureAdConfig["TenantId"];

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId), "El 'ClientId' no puede estar vacío. Revisa appsettings.json.");
            if (string.IsNullOrEmpty(clientSecret))
                throw new ArgumentNullException(nameof(clientSecret), "El 'ClientSecret' no puede estar vacío. Revisa appsettings.json.");
            if (string.IsNullOrEmpty(tenantId))
                throw new ArgumentNullException(nameof(tenantId), "El 'TenantId' no puede estar vacío. Revisa appsettings.json.");

            _app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
                .Build();
        }

        public async Task<string> GetApplicationTokenAsync()
        {
            var sharePointConfig = _configuration.GetSection("PowerAutomate");
            var scope = sharePointConfig["Scope"];

            if (string.IsNullOrEmpty(scope))
                throw new ArgumentNullException(nameof(scope), "El 'Scope' de PowerAutomate no puede estar vacío. Revisa appsettings.json.");

            var scopes = new[] { scope };

            AuthenticationResult result = await _app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }
    }
}
