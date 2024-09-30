using Microsoft.AspNetCore.Mvc;
using st10158209.Services;
using st10158209.Models;
using Microsoft.AspNetCore.WebUtilities;


namespace st10158209.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlobService _blobService;
        private readonly TableService _tableService;
        private readonly QueueService _queueService;
        private readonly FileService _fileService;
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(BlobService blobService, TableService tableService, QueueService queueService, FileService fileService, HttpClient httpClient, ILogger<HomeController> logger)
        {
            _blobService = blobService;
            _tableService = tableService;
            _queueService = queueService;
            _fileService = fileService;
            _httpClient = httpClient;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult file()
        {
            return View();
        }
        public IActionResult order()
        {
            return View();
        }
        public IActionResult contract()
        {
            return View();
        }
        public IActionResult thank_you()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please upload a valid file.");
                return RedirectToAction("order"); // Redirect with an error message if needed
            }

            try
            {
                // Prepare the URL for the Azure Function
                var functionUrl = "https://s-t-10158209.azurewebsites.net/api/UploadFile?code=fwXrFseYXF8KUmhRIzpik9YJDxYFJN0B0J6DrYaMYKhoAzFu8QrYxQ%3D%3D";

                // Create a new MultipartFormDataContent for the request
                using var content = new MultipartFormDataContent();

                // Add the file to the content
                using var stream = file.OpenReadStream();
                content.Add(new StreamContent(stream), "file", file.FileName);

                // Add any additional parameters if required (like shareName)
                content.Add(new StringContent("images"), "shareName");

                // Send the POST request to the Azure Function
                var response = await _httpClient.PostAsync(functionUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Optionally handle the success response
                    return RedirectToAction("order");
                }
                else
                {
                    ModelState.AddModelError("File", "Error uploading the file.");
                    return RedirectToAction("order");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you might want to log this to a logging service)
                ModelState.AddModelError("File", "An error occurred while uploading the file.");
                return RedirectToAction("order");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomerProfile(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                // Function URL (replace with your actual function app URL)
                var functionUrl = "https://s-t-10158209.azurewebsites.net/api/StoreTableInfo?code=vecQX9fO17f0ZfYTjyLM9fescx-xraNU_uPSWTPLA3vqAzFu_grMlg%3D%3D";

                // Prepare query parameters
                var queryParams = new Dictionary<string, string>
                {
                    { "tableName", "CustomerProfileTable" },  // Specify the table name
                    { "partitionKey", profile.PartitionKey },
                    { "rowKey", profile.RowKey },
                    { "data", profile.ToString() }  // You can serialize the object or pass specific data
                };

                // Build the full URL with query string
                var requestUrl = QueryHelpers.AddQueryString(functionUrl, queryParams);

                // Make the POST request to the Azure Function
                var response = await _httpClient.PostAsync(requestUrl, null);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("file"); // Redirect to a success page or action
                }
                else
                {
                    // Handle the error (logging, error message, etc.)
                    return RedirectToAction("Error"); // Handle error scenario
                }
            }

            // If model validation fails, return the same view with the profile data
            return RedirectToAction("file");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder(string orderId)
        {
            var functionUrl = "https://s-t-10158209.azurewebsites.net/api/ProcessQueueMessage?code=NCxBConnOtCEjc3lkcUjzKNExru0QYyn4aggDCg2cptTAzFue-FA4g%3D%3D";

            // await _httpClient.PostAsync(functionUrl, );
            // Log order processing start
            _logger.LogInformation($"Processing order {orderId}");

            // Create the content for the POST request (if needed)
            var content = new StringContent("");  // Update with actual content if needed

            var response = await _httpClient.PostAsync(functionUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to post order {orderId} to function. StatusCode: {response.StatusCode}");
                return StatusCode((int)response.StatusCode, "Error processing order");

            }
            else
            {
                _logger.LogInformation($"Order {orderId} successfully posted to function");
            }

            await _queueService.SendMessageAsync("order", $"Processing order={orderId}");

            // Log completion of the order processing
            _logger.LogInformation($"Order {orderId} processing completed.");
            return RedirectToAction("contract");

            // await _queueService.SendMessageAsync("order", $"Processing order={orderId}");
            // return RedirectToAction("contract");
        }

        [HttpPost]
        public async Task<IActionResult> UploadContract(IFormFile file)
        {

            try
            {
                // Prepare the URL for the Azure Function
                var functionUrl = "https://s-t-10158209.azurewebsites.net/api/UploadBlob?code=Jf6ZQGx6N8b1JLws9p6KZY7MUUAhSNr4ch0JlcoNYMJKAzFuWnbp4A%3D%3D"; // Update with your actual function URL

                // Create a new MultipartFormDataContent for the request
                using var content = new MultipartFormDataContent();

                // Add the file to the content
                using var stream = file.OpenReadStream();
                content.Add(new StreamContent(stream), "file", file.FileName);

                // Add any additional parameters if required (like shareName or other data)
                content.Add(new StringContent("contracts"), "shareName"); // Specify the share name
                content.Add(new StringContent(file.FileName), "fileName"); // Specify the file name

                // Send the POST request to the Azure Function
                var response = await _httpClient.PostAsync(functionUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Optionally handle the success response
                    return RedirectToAction("thank_you");
                }
                else
                {
                    ModelState.AddModelError("File", "Error uploading the contract.");
                    return RedirectToAction("thank_you");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you might want to log this to a logging service)
                ModelState.AddModelError("File", "An error occurred while uploading the contract.");
                return RedirectToAction("thank_you");
            }
        }
    }
}

//McCall, B., 2024. CLDV_SemesterTwo_Byron. [online] GitHub.Available at: https://github.com/ByronMcCallLecturer/CLDV_SemesterTwo_Byron [Accessed 29 August 2024].