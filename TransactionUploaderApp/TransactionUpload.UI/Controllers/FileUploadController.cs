using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace TransactionUpload.UI.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FileUploadController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please select a file.";
                return View("Upload");
            }

            // Call the API to upload the file
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(5);
            var apiUrl = "https://localhost:7010/api/FileUploader/Upload"; // Your API URL

            using (var content = new MultipartFormDataContent())
            {
                using (var streamContent = new StreamContent(file.OpenReadStream()))
                {
                    streamContent.Headers.Add("Content-Type", file.ContentType);
                    content.Add(streamContent, "file", file.FileName);

                    var response = await client.PostAsync(apiUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return Ok();
                       // ViewBag.Message = "File uploaded successfully!";
                    }
                    else
                    {
                        return BadRequest(response);
                    }
                }
            }

            return View("Index");
        }
    }
}
