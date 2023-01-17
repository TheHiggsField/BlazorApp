using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Azure;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;
using System.Threading.Tasks;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Azure.Security.KeyVault.Secrets;

namespace AzureADLogin
{
    class PseudoPrincipal
    {
        private const string _clientId = "c79dd058-f5e8-4e61-8ce1-5dc194ba77b1";
        private const string _tenantId = "fdaef475-4076-49a6-afec-94a2a319d8db";
        private SecretClient keyVaultClient;
        Azure.Core.TokenCredential default_credential;

        public PseudoPrincipal(Azure.Core.TokenCredential _default_credential, SecretClient _keyVaultClient)
        {
            default_credential = _default_credential;
            keyVaultClient = _keyVaultClient;
        }

        private string[] scopes = { "User.Read", "https://csb100320026571cde5.blob.core.windows.net/user_impersonation" } ;

        
        IConfidentialClientApplication? app {get; set;} = null;

        public async Task<Uri> GetAuthorizationRequestUrl(string uriString)
        {           
            System.Uri uri = new System.Uri(uriString);
            //Microsoft.Identity.Client.IAccount derp = GetAuthorizationRequestUrlParameterBuilder
            app = ConfidentialClientApplicationBuilder
                .Create(_clientId)
                .WithTenantId(_tenantId)
                .WithClientSecret(keyVaultClient.GetSecret("7a247311-5d2a-4d13-8187-2e6fd9e17994").Value.Value)
                .WithRedirectUri(uri.GetLeftPart(UriPartial.Path) )
                .Build();
            
            //var accounts = await app.GetAccountsAsync();
            Dictionary<string,string> extraParameters = new Dictionary<string,string>()
                                                        {
                                                            //{"response_mode","form_post" },
                                                            {"state", Guid.NewGuid().ToString()}
                                                        };

            Console.WriteLine(extraParameters["state"]);

            var AuthURI = await app.GetAuthorizationRequestUrl( scopes)
                                    .WithExtraQueryParameters(extraParameters)
                                    .ExecuteAsync();
            return AuthURI;
        }
        
        public string? GetCodeFromUri(string uriString)
        {   
            

            System.Uri uri = new System.Uri(uriString);
            app = ConfidentialClientApplicationBuilder
                .Create(_clientId)
                .WithTenantId(_tenantId)
                .WithClientSecret(keyVaultClient.GetSecret("7a247311-5d2a-4d13-8187-2e6fd9e17994").Value.Value)
                .WithRedirectUri(uri.GetLeftPart(UriPartial.Path))
                .Build();
            return QueryHelpers.ParseQuery(uri.Query)?["code"];
        }

        public async Task<AuthorizationCodeCredential?> GetAuthorizationCodeCredential(string uriString)
        {
            System.Uri uri = new System.Uri(uriString);
            
            string? code;
            
            try 
            {
                code = QueryHelpers.ParseQuery(uri.Query)?["code"];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }

            if(code == null)
            {
                Console.WriteLine("code is null :-/");
                return null;
            }
        
            AuthorizationCodeCredentialOptions options = new AuthorizationCodeCredentialOptions();

            options.RedirectUri = new System.Uri(uri.GetLeftPart(UriPartial.Path));
            var client_secret = await keyVaultClient.GetSecretAsync("7a247311-5d2a-4d13-8187-2e6fd9e17994");
            AuthorizationCodeCredential authZCredential = new AuthorizationCodeCredential(_tenantId, 
                                                                                            _clientId, 
                                                                                            client_secret.Value.Value, 
                                                                                            code,
                                                                                            options
                                                                                        );

            Console.WriteLine(authZCredential);
            return authZCredential;

        }

        public async Task<AuthenticationResult?> GetTokenFromCode(string? authZCode)
        {
            
            
            if (app == null || authZCode == null )
            {
                Console.WriteLine($"Token:app = {app}\nauthZCode\t{authZCode}");
                return null;
            }
            
            AuthenticationResult result = await app.AcquireTokenByAuthorizationCode(scopes, authZCode ).ExecuteAsync();
            
            Console.WriteLine($"Account:\t{result.Account}\n Token:\t{result.AccessToken}");
            var derp = result.CreateAuthorizationHeader();
            return result;
        }
    }
}