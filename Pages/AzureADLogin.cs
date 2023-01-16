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

namespace AzureADLogin
{
    class PseudoPrincipal
    {
        private const string _clientId = "c79dd058-f5e8-4e61-8ce1-5dc194ba77b1";
        private const string _tenantId = "fdaef475-4076-49a6-afec-94a2a319d8db";
        private const string _clientSecret = "NJR8Q~mw9R1cB5SjsnNaoI1sB15jEai8m9tOtbg1";
        private string[] scopes = { "User.Read", "https://csb100320026571cde5.blob.core.windows.net/user_impersonation" } ;

        
        IConfidentialClientApplication? app {get; set;} = null;

        public async Task<Uri> GetAuthorizationRequestUrl(string uriString)
        {           
            System.Uri uri = new System.Uri(uriString);
            //Microsoft.Identity.Client.IAccount derp = GetAuthorizationRequestUrlParameterBuilder
            app = ConfidentialClientApplicationBuilder
                .Create(_clientId)
                .WithTenantId(_tenantId)
                .WithClientSecret("NJR8Q~mw9R1cB5SjsnNaoI1sB15jEai8m9tOtbg1")
                .WithRedirectUri(uri.GetLeftPart(UriPartial.Path) )
                .Build();
            
            //var accounts = await app.GetAccountsAsync();

            var AuthURI = await app.GetAuthorizationRequestUrl( scopes).ExecuteAsync();
            return AuthURI;
        }
        
        public string? GetCodeFromUri(string uriString)
        {   
            

            System.Uri uri = new System.Uri(uriString);
            app = ConfidentialClientApplicationBuilder
                .Create(_clientId)
                .WithTenantId(_tenantId)
                .WithClientSecret(_clientSecret)
                .WithRedirectUri(uri.GetLeftPart(UriPartial.Path))
                .Build();
            return QueryHelpers.ParseQuery(uri.Query)?["code"];
        }

        public AuthorizationCodeCredential GetAuthorizationCodeCredential(string uriString)
        {
            System.Uri uri = new System.Uri(uriString);

            AuthorizationCodeCredentialOptions options = new AuthorizationCodeCredentialOptions();

            options.RedirectUri = new System.Uri(uri.GetLeftPart(UriPartial.Path));

            AuthorizationCodeCredential authZCredential = new AuthorizationCodeCredential(_tenantId, 
                                                                                            _clientId, 
                                                                                            _clientSecret, 
                                                                                            QueryHelpers.ParseQuery(uri.Query)?["code"],
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