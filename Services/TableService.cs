

using Azure.Data.Tables;
using st10158209.Models;


namespace st10158209.Services
{
    public class TableService
    {
        private readonly TableClient _tableClient;

        public TableService(IConfiguration configuration)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=cldvst10158209;AccountKey=UmRFyrMs7YHblP0At2d2+nR+j9YaVWpPjskIo0SsrA1NjO6tmU1l31UV94OILoQ6H9b66R2rHIXD+AStEVi+Hg==;EndpointSuffix=core.windows.net";
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient("CustomerProfiles");
            _tableClient.CreateIfNotExists();
        }

        public async Task AddEntityAsync(CustomerProfile profile)
        {
            await _tableClient.AddEntityAsync(profile);
        }
    }
}