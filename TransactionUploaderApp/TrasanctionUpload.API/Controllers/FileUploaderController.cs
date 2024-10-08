using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionUpload.Application.Interface;

namespace TrasanctionUpload.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploaderController : ControllerBase
    {
        private readonly IUploadFileService _uploadFileService;
        public FileUploaderController(IUploadFileService uploadFileService)
        {
            _uploadFileService = uploadFileService;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file.Length > 1048576) // Check file size limit (1MB)
            {
                return BadRequest("File size exceeds 1 MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".csv" && extension != ".xml")
            {
                return BadRequest("Unknown format");
            }

            using var stream = new StreamReader(file.OpenReadStream());
            var invalidData = await _uploadFileService.FileProcess(stream, extension);
            if (invalidData.Any())
            {
                return BadRequest(new { message = "Invalid data found in the file.", invalidData = invalidData });
            }

            return Ok("File processed successfully");
        }
        [HttpGet("currency")]
        public async Task<IActionResult> GetByCurrency(string currency)
        {
            var transactions = await _uploadFileService.GetByCurrency(currency);
            return Ok(transactions);
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var transactions = await _uploadFileService.GetByDateRange(startDate, endDate);
            return Ok(transactions);
        }
        [HttpGet("status")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var transactions = await _uploadFileService.GetByStatus(status);
            return Ok(transactions);
           
        }
    }

}
