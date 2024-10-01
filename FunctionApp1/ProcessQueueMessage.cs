using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


namespace FunctionApp1
{
    public static class ProcessQueueMessage
    {
        [Function("ProcessQueueMessage")]
        public static async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
          ILogger log)
        {
            log.LogInformation("Processing queue message...");
            string queueName = req.Query["queueName"];
            string message = req.Query["message"];

            if (string.IsNullOrEmpty(queueName) || string.IsNullOrEmpty(message))
            {
                return new BadRequestObjectResult("Queue name and message must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("DefaultEndpointsProtocol=https;AccountName=cldvst10158209;AccountKey=UmRFyrMs7YHblP0At2d2+nR+j9YaVWpPjskIo0SsrA1NjO6tmU1l31UV94OILoQ6H9b66R2rHIXD+AStEVi+Hg==;EndpointSuffix=core.windows.net");
            if (string.IsNullOrEmpty(connectionString))
            {
                log.LogError("Storage connection string is missing or invalid.");
                return new StatusCodeResult(500); // Internal Server Error
            }

            var queueServiceClient = new QueueServiceClient(connectionString);
            var queueClient = queueServiceClient.GetQueueClient(queueName);

            log.LogInformation($"Ensuring queue {queueName} exists...");
            await queueClient.CreateIfNotExistsAsync();

            log.LogInformation($"Adding message to queue {queueName}: {message}");
            await queueClient.SendMessageAsync(message);

            log.LogInformation("Message successfully added to queue.");
            return new OkObjectResult("Message added to queue");
        }
    }
}
