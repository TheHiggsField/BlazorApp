using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;
using System.Threading.Tasks;


namespace Storage
{
    
    
    class Blobs
    {
        BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri("https://storagethf.blob.core.windows.net"),
                                                                    new DefaultAzureCredential());
        
        public async static Task<List<String>> MyListBlobsAsync(string accountName, string containerName)
        {
            // Construct the blob container endpoint from the arguments.
            string containerEndpoint = string.Format("https://{0}.blob.core.windows.net/{1}",
                                                        accountName,
                                                        containerName);

            // Get a credential and create a service client object for the blob container.
            BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                                    new DefaultAzureCredential());

            List<string> BlobList = new List<string>();

            try
            {
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    BlobList.Add(blobItem.Name);
                }

                return BlobList;
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}