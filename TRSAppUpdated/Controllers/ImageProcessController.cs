using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;

namespace TRSAppUpdated.Controllers
{
    [Authorize]
    public class ImageProcessController : Controller
    {
        private readonly IOcrService _ocrService;
        private readonly ILogger<ImageProcessController> _logger;

        public ImageProcessController(IOcrService ocrService, ILogger<ImageProcessController> logger)
        {
            _ocrService = ocrService;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("ocr-limit")]
        public async Task<IActionResult> ExtractTextFromImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded." });
            }

            if (!AppHelper.IsValidImageFile(file, out var imgError))
            {
                return BadRequest(new { message = imgError ?? "Invalid image file format." });
            }

            try
            {
                string extractedText = await _ocrService.ExtractTextFromImageAsync(file);
                return Ok(new { text = extractedText });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing OCR in ExtractTextFromImage");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing the image." });
            }
        }
    }
}

