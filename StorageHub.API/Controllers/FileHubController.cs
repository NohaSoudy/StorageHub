using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SotrageHub.Application;
using StorageHub.API.Controllers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace fileHubServiceHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileHubController : ControllerBase
    {

        private readonly IFileHubService _fileHubService;
        private readonly ILogger<FileHubController> _logger;
        public FileHubController(IFileHubService fileHubService, ILogger<FileHubController> logger)
        {
            _fileHubService = fileHubService;
            _logger = logger;
        }

        #region UPLOAD
        [Route("Upload")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            int _return = await _fileHubService.SaveFileAsync(file);

            if (_return == 0) return StatusCode(500, "Failed to save file.");

            return StatusCode(200, "Uplaod file Successfully");
        }
        #endregion

        #region DOWNLOAD
        [HttpGet("Download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var file = await _fileHubService.GetFileById(id);
            if (file == null) return NotFound();

            var fileBytes = await _fileHubService.GetFileStreamAsync(file.FilePath);
            if (fileBytes == null) return NotFound();

            return File(fileBytes, file.ContentType, file.FileName);
        }
        #endregion

        #region DELETE
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _fileHubService.DeleteFile(id))
            {
                return StatusCode(200, "Delete file Successfully");
            }

            return StatusCode(500, "Failed to delete file.");
        }

        [HttpDelete("DeletePermanently/{id}")]
        public async Task<IActionResult> DeletePermanently(Guid id)
        {
            if (await _fileHubService.DeleteFilePermanently(id))
            {
                return StatusCode(200, "Delete file Successfully");
            }

            return StatusCode(500, "Failed to delete file.");
        }
        #endregion

        #region GET ALL FILES 
        [HttpGet]
        [Route("ListAllFiles")]
        public IActionResult List()
        {
            var files = _fileHubService.GetAllFiles();
            return Ok(files.Result.ToList());
        }
        #endregion
    }

}




