using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public static class UploadBlob
    {
        [Function("UploadBlob")]
        public static async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("Processing upload blob request...");

            string containerName = req.Query["containerName"];
            string blobName = req.Query["blobName"];

            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
            {
                log.LogError("Container name and blob name must be provided.");
                return new BadRequestObjectResult("Container name and blob name must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            if (string.IsNullOrEmpty(connectionString))
            {
                log.LogError("Blob storage connection string is missing.");
                return new StatusCodeResult(500); // Internal Server Error
            }

            try
            {
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();
                var blobClient = containerClient.GetBlobClient(blobName);

                using var stream = req.Body;
                await blobClient.UploadAsync(stream, true);

                log.LogInformation($"Blob '{blobName}' uploaded to container '{containerName}'.");
                return new OkObjectResult("Blob uploaded");
            }
            catch (Exception ex)
            {
                log.LogError($"Error uploading blob: {ex.Message}");
                return new StatusCodeResult(500); // Internal Server Error
            }
        }
    }
}
