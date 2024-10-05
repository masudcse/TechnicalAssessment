using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            await _uploadFileService.FileProcess(stream,extension);
            

           
            //else
            //{
            //    return BadRequest("Unsupported file format");
            //}

            //if (transactions == null || !transactions.Any())
            //{
            //    return BadRequest("No valid transactions found.");
            //}
            return Ok("File processed successfully");
        }

    }
}
