using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace AzureADLogin
{
    class PseudoPrincipal
    {
        private const string _clientId = "c79dd058-f5e8-4e61-8ce1-5dc194ba77b1";
        private const string _tenantId = "fdaef475-4076-49a6-afec-94a2a319d8db";

        public static async Task LoginAD()
        {
            var app = PublicClientApplicationBuilder
                .Create(_clientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
                .WithRedirectUri("http://localhost/")
                .Build();
            string[] scopes = { "https://csb100320026571cde5.blob.core.windows.net/user_impersonation" };
            AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            
            Console.WriteLine($"Token:\t{result.AccessToken}");
        }
    }
}