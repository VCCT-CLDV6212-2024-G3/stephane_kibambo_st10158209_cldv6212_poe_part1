using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public static class UploadFile
    {
        [Function("UploadFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {
            var log = executionContext.GetLogger("UploadFile");
            log.LogInformation("Processing file upload request...");

            // Extract query parameters for shareName and fileName
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string shareName = query["shareName"];
            string fileName = query["fileName"];

            if (string.IsNullOrEmpty(shareName) || string.IsNullOrEmpty(fileName))
            {
                log.LogError("Share name or file name not provided.");
                return new BadRequestObjectResult("Share name and file name must be provided.");
            }

            // Log file and share details
            log.LogInformation($"Share Name: {shareName}, File Name: {fileName}");

            // Get the Azure Files connection string from environment variables
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            if (string.IsNullOrEmpty(connectionString))
            {
                log.LogError("Azure storage connection string is missing.");
                return new StatusCodeResult(500); // Internal Server Error
            }

            // Initialize the ShareServiceClient
            var shareServiceClient = new ShareServiceClient(connectionString);
            var shareClient = shareServiceClient.GetShareClient(shareName);

            // Ensure the share exists
            await shareClient.CreateIfNotExistsAsync();
            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(fileName);

            // Log file upload start
            log.LogInformation("Uploading file...");

            // Upload the file from the request body stream
            using var stream = req.Body;
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadAsync(stream);

            // Log file upload success
            log.LogInformation("File uploaded successfully.");

            // Return a success response
            return new OkObjectResult("File uploaded to Azure Files");
        }
    }
}