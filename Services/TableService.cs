
using Azure.Data.Tables;
using st10158209.Models;
using System.Threading.Tasks;
using Azure;

namespace st10158209.Services
{
    public class TableService
    {
        private readonly TableClient _tableClient;

        public TableService()
        {
            // Connection string is hardcoded here (not recommended for production)
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=cldvst10158209;AccountKey=UmRFyrMs7YHblP0At2d2+nR+j9YaVWpPjskIo0SsrA1NjO6tmU1l31UV94OILoQ6H9b66R2rHIXD+AStEVi+Hg==;EndpointSuffix=core.windows.net";

            // Initialize the TableServiceClient
            var serviceClient = new TableServiceClient(connectionString);

            // Get the TableClient for a specific table
            _tableClient = serviceClient.GetTableClient("CustomerProfiles");

            // Ensure the table exists
            _tableClient.CreateIfNotExists();
        }

        // Add a CustomerProfile entity to the table
        public async Task AddEntityAsync(CustomerProfile profile)
        {
            try
            {
                await _tableClient.AddEntityAsync(profile);
            }
            catch (RequestFailedException ex)
            {
                // Log the exception or handle it accordingly
                throw new Exception("Failed to add entity to the table.", ex);
            }
        }
    }
}
//McCall, B., 2024. CLDV_SemesterTwo_Byron. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_SemesterTwo_Byron [Accessed 29 August 2024].