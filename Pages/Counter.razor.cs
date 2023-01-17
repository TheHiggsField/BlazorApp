using BlazorApp;
using Azure.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Azure.Security.KeyVault.Secrets;


namespace BlazorApp.Pages {


    public partial class CounterBase : ComponentBase{

        public int currentCount {get;set;} = 0;
        [Parameter]
        public Azure.Identity.AuthorizationCodeCredential? authZCredential {get;set;}
        [Parameter]
        public string blobsString {get;set;} = "Has not searched for blobs yet.";
        [Parameter]
        public string? AuthZCode { get; set; } = null;
        
        [Parameter]
        public string redirectURI { get; set; } = "";
        private Microsoft.Identity.Client.AuthenticationResult? token;

        [Inject]
        private NavigationManager MyNavigationManager {get;set;} = default!;

        [Inject]
        private Azure.Core.TokenCredential default_credential {get;set;} = default!;

        [Inject]
        private SecretClient keyVaultClient {get;set;} = default!;

        private AzureADLogin.PseudoPrincipal psPrincipal = default!;

        public CounterBase()
        {
            Console.WriteLine(default_credential);
            Console.WriteLine(MyNavigationManager);

        }

        protected override async Task OnInitializedAsync()
        {   
            await base.OnInitializedAsync();
            psPrincipal = new AzureADLogin.PseudoPrincipal(default_credential, keyVaultClient);
            authZCredential = authZCredential ?? await psPrincipal.GetAuthorizationCodeCredential(MyNavigationManager.Uri);
            Console.WriteLine("This is OnInitializedAsync" );

        }

        public void SetValue(string? Value)
        {
            redirectURI = Value != null ? Value : "Null :-|";    
        }

        public async void GetToken()
        {

            AuthZCode = psPrincipal.GetCodeFromUri(MyNavigationManager.Uri);
            
            if (AuthZCode == null){

                return;
            }

            token = await psPrincipal.GetTokenFromCode(AuthZCode);

            if (token == null){

                blobsString = "null :-|";
                return;
            }
            
            blobsString = token.CreateAuthorizationHeader();
            

            await Storage.Blobs.GetBlob("csb100320026571cde5", "randomstuff", "Les_problems.txt", token);
            return;
            
        }

        public async void Login()
        {
            redirectURI = MyNavigationManager.Uri;
            blobsString = $"redirectURI = \n {redirectURI}";
            
            var redirect  = await psPrincipal.GetAuthorizationRequestUrl(redirectURI);
            
            MyNavigationManager.NavigateTo(redirect.ToString(), true);
            
        }

        public async Task ListBlobsAsync(string accountName, string containerName, Azure.Core.TokenCredential? credential)
        {

            if (credential == null){
                
                blobsString = "Credential is null :-/";
                return;
            }

            List<string>? blobNames = null;

            try
            {
                blobNames = await Storage.Blobs.ListBlobsAsyncToken(accountName, containerName, credential);

            }
            catch (AuthenticationFailedException)
            {
                credential = null;
                blobsString = "AuthenticationFailedException.";
                return;
            }
            
            if ( blobNames != null && blobNames.Any() )
            {
                
                blobsString = "";
                foreach (string blobName in blobNames)
                {
                    blobsString += blobName + ",\t"; 
                }
            }
            else
            {
                blobsString = "No blobs found.";
            }
            
            StateHasChanged();
            
        }
        
        public async void ListUserBlobsAsync()
        {
            await ListBlobsAsync("csb100320026571cde5", "randomstuff", authZCredential);
        }

        public async void ListDefaultBlobsAsync()
        {
            await ListBlobsAsync("storagethf", "randomstuff", default_credential);
        }
    }
}