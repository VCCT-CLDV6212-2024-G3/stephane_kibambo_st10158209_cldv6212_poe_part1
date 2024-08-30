using Azure.Storage.Blobs;



namespace st10158209.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=cldvst10158209;AccountKey=UmRFyrMs7YHblP0At2d2+nR+j9YaVWpPjskIo0SsrA1NjO6tmU1l31UV94OILoQ6H9b66R2rHIXD+AStEVi+Hg==;EndpointSuffix=core.windows.net");
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream content)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(content, true);
        }
    }

}
