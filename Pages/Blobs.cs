using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Storage
{
    
    
    class Blobs
    {
        BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri("https://storagethf.blob.core.windows.net"),
                                                                    new DefaultAzureCredential());
        
            
        private const string _clientId = "c79dd058-f5e8-4e61-8ce1-5dc194ba77b1";
        private const string _tenantId = "fdaef475-4076-49a6-afec-94a2a319d8db";
        private const string _clientSecret = "NJR8Q~mw9R1cB5SjsnNaoI1sB15jEai8m9tOtbg1";

        public static async Task TestBlob(string accountName, string containerName, AuthenticationResult token)
        {

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("x-ms-version", "2021-06-08");
            //client.DefaultRequestHeaders.Add("Authorization", token.CreateAuthorizationHeader());
            
            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, 
                string.Format(
                    $"https://{accountName}.blob.core.windows.net/?restype=service&comp=userdelegationkey"
                )
            );

            request.Headers.Add("Authorization", token.CreateAuthorizationHeader());
            request.Content = new StringContent(
                "<KeyInfo><Start>2023-01-08T22:21:27Z</Start><Expiry>2023-01-20T22:21:27Z</Expiry></KeyInfo>"
            );

            HttpResponseMessage respone = await client.SendAsync(request);
            var content = await respone.Content.ReadAsStringAsync();
            Console.WriteLine(content);
    

        }

        
        public static async Task GetBlob(string accountName, string containerName, string blobName, AuthenticationResult token)
        {

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("x-ms-version", "2021-06-08");
            //client.DefaultRequestHeaders.Add("Authorization", token.CreateAuthorizationHeader());
            
            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, 
                string.Format(
                    $"https://{accountName}.blob.core.windows.net/{containerName}/{blobName}"
                )
            );

            request.Headers.Add("Authorization", token.CreateAuthorizationHeader());

            HttpResponseMessage respone = await client.SendAsync(request);
            var content = await respone.Content.ReadAsStringAsync();
            Console.WriteLine(content);
    

        }

        // Could also use new DefaultAzureCredential() to get the a TokenCredential corresponding to the server running the codes Azure RBAC identity.
        public async static Task<List<String>> ListBlobsAsyncToken(string accountName, string containerName, Azure.Core.TokenCredential authZCredential)
        {
            
            // Construct the blob container endpoint from the arguments.
            System.Uri containerEndpoint = new System.Uri(string.Format("https://{0}.blob.core.windows.net/{1}",
                                                            accountName,
                                                            containerName
                                                ));
        
            // Get a credential and create a service client object for the blob container.
            BlobContainerClient containerClient = new BlobContainerClient(containerEndpoint, authZCredential );

            List<string> BlobList = new List<string>();

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                BlobList.Add(blobItem.Name);
            }

            return BlobList;
        }


    }
}