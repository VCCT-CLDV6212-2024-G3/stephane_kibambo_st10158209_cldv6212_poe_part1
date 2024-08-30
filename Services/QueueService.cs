using Azure.Storage.Queues;



namespace st10158209.Services
{
    public class QueueService
    {
        private readonly QueueServiceClient _queueServiceClient;

        public QueueService(IConfiguration configuration)
        {
            _queueServiceClient = new QueueServiceClient("DefaultEndpointsProtocol=https;AccountName=cldvst10158209;AccountKey=UmRFyrMs7YHblP0At2d2+nR+j9YaVWpPjskIo0SsrA1NjO6tmU1l31UV94OILoQ6H9b66R2rHIXD+AStEVi+Hg==;EndpointSuffix=core.windows.net");
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }
    }

}
