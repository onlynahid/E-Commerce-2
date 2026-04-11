using AYYUAZ.APP.Application.Interfaces;
using AYYUAZ.APP.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AYYUAZ.APP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileStorageController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        public FileStorageController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }
        /// <param name="request">
        [HttpPost("upload")]
        [RequireAdmin]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<object>> UploadFile([FromForm] FileUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded." });
            }

            if (!_fileStorageService.IsImageFile(request.File))
            {
                return BadRequest(new { message = "Only image files are allowed." });
            }

            var folder = string.IsNullOrEmpty(request.Folder) ? "products" : request.Folder;
            var imageUrl = await _fileStorageService.UploadImageAsync(request.File, folder);
            
            return Ok(new 
            { 
                success = true,
                message = "File uploaded successfully.",
                imageUrl,
                fileName = request.File.FileName,
                contentType = request.File.ContentType,
                size = request.File.Length
            });
        }

        /// <summary>
        /// Upload multiple image files
        /// </summary>
        /// <param name="request">Multiple files upload request</param>
        /// <returns>Upload results for all files</returns>
        [HttpPost("upload-multiple")]
        [RequireAdmin]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<object>> UploadMultipleFiles([FromForm] MultipleFileUploadRequest request)
        {
            if (request.Files == null || request.Files.Count == 0)
            {
                return BadRequest(new { message = "No files uploaded." });
            }

            var uploadResults = new List<object>();
            var errors = new List<string>();
            var folder = string.IsNullOrEmpty(request.Folder) ? "products" : request.Folder;

            foreach (var file in request.Files)
            {
                if (file == null || file.Length == 0)
                {
                    errors.Add($"File {file?.FileName ?? "unknown"} is empty.");
                    continue;
                }

                if (!_fileStorageService.IsImageFile(file))
                {
                    errors.Add($"File {file.FileName} is not a valid image file.");
                    continue;
                }

                var imageUrl = await _fileStorageService.UploadImageAsync(file, folder);
                uploadResults.Add(new
                {
                    fileName = file.FileName,
                    imageUrl,
                    contentType = file.ContentType,
                    size = file.Length,
                    success = true
                });
            }

            return Ok(new
            {
                uploadedFiles = uploadResults,
                errors = errors,
                totalUploaded = uploadResults.Count,
                totalErrors = errors.Count
            });
        }
        /// <summary>
        /// Delete an uploaded file
        /// </summary>
        /// <param name="imageUrl">URL of the image to delete</param>
        /// <returns>Deletion result</returns>
        [HttpDelete]
        [RequireAdmin]
        public async Task<ActionResult<object>> DeleteFile([FromQuery] [Required] string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest(new { message = "Image URL is required." });
            }

            var result = await _fileStorageService.DeleteImageAsync(imageUrl);
            
            if (result)
            {
                return Ok(new 
                { 
                    success = true,
                    message = "File deleted successfully.",
                    imageUrl 
                });
            }
            else
            {
                return NotFound(new 
                { 
                    success = false,
                    message = "File not found or could not be deleted.",
                    imageUrl 
                });
            }
        }

        /// <summary>
        /// Validate if a file is a valid image
        /// </summary>
        /// <param name="request">File validation request</param>
        /// <returns>Validation result</returns>
        [HttpPost("validate")]
        [Consumes("multipart/form-data")]
        public ActionResult<object> ValidateFile([FromForm] FileValidationRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { message = "No file provided." });
            }

            var isValid = _fileStorageService.IsImageFile(request.File);
            
            return Ok(new
            {
                fileName = request.File.FileName,
                contentType = request.File.ContentType,
                size = request.File.Length,
                isValid,
                message = isValid ? "File is a valid image." : "File is not a valid image."
            });
        }
    }
    public class FileUploadRequest
    {     
        [Required]
        public IFormFile File { get; set; } = null!;

        public string? Folder { get; set; }
    }
    public class MultipleFileUploadRequest
    {
        [Required]
        public List<IFormFile> Files { get; set; } = new();
        public string? Folder { get; set; }
    }
    public class FileValidationRequest
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}