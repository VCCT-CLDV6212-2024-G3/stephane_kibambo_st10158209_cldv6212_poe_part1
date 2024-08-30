using Microsoft.AspNetCore.Mvc;
using st10158209.Services;
using st10158209.Models;


namespace st10158209.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlobService _blobService;
        private readonly TableService _tableService;
        private readonly QueueService _queueService;
        private readonly FileService _fileService;

        public HomeController(BlobService blobService, TableService tableService, QueueService queueService, FileService fileService)
        {
            _blobService = blobService;
            _tableService = tableService;
            _queueService = queueService;
            _fileService = fileService;
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
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                await _blobService.UploadBlobAsync("images", file.FileName, stream);
            }
            return RedirectToAction("order");
        }
        [HttpPost]
        public async Task<IActionResult> AddCustomerProfile(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                await _tableService.AddEntityAsync(profile);
            }
            return RedirectToAction("file");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder(string orderId)
        {
            await _queueService.SendMessageAsync("order", $"Processing order={orderId}");
            return RedirectToAction("contract");
        }

        [HttpPost]
        public async Task<IActionResult> UploadContract(IFormFile file)
        {
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                await _fileService.UploadFileAsync("contracts", file.FileName, stream);
            }
            return RedirectToAction("thank_you");
        }
    }
}

