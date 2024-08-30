
using Azure.Storage.Files.Shares;


namespace st10158209.Services
{
    public class FileService
    {
        private readonly ShareServiceClient _shareServiceClient;

        public FileService(IConfiguration configuration)
        {
            _shareServiceClient = new ShareServiceClient("DefaultEndpointsProtocol=https;AccountName=cldvst10158209;AccountKey=UmRFyrMs7YHblP0At2d2+nR+j9YaVWpPjskIo0SsrA1NjO6tmU1l31UV94OILoQ6H9b66R2rHIXD+AStEVi+Hg==;EndpointSuffix=core.windows.net");
        }

        public async Task UploadFileAsync(string shareName, string fileName, Stream content)
        {
            var shareClient = _shareServiceClient.GetShareClient(shareName);
            await shareClient.CreateIfNotExistsAsync();
            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(fileName);
            await fileClient.CreateAsync(content.Length);
            await fileClient.UploadAsync(content);
        }
    }
}
