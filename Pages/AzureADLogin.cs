using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace AzureADLogin
{
    class PseudoPrincipal
    {
        private const string _clientId = "e1256751-d79e-4efc-8c16-90676b30091e";
        private const string _tenantId = "635aa01e-f19d-49ec-8aed-4b2e4312a627";

        public static async Task LoginAD()
        {
            var app = PublicClientApplicationBuilder
                .Create(_clientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
                .WithRedirectUri("http://localhost:5082/counter")
                .Build(); 
            string[] scopes = {  };
            AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();

            Console.WriteLine($"Token:\t{result.AccessToken}");
        }
    }
}