using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tesseract;
using TRSAppUpdated.Helpers;

namespace TRSAppUpdated.Controllers
{
    [Authorize]
    public class ImageProcessController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ImageProcessController> _logger;

        public ImageProcessController(IWebHostEnvironment webHostEnvironment, ILogger<ImageProcessController> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExtractTextFromImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (!AppHelper.IsValidImageFile(file, out var imgError))
            {
                return BadRequest(imgError ?? "Invalid image file format.");
            }

            string filePath = null;
            try
            {
                string extractedText;
                // Save the file temporarily
                filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Use Tesseract to extract text
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (var engine = new TesseractEngine(path + Path.DirectorySeparatorChar + "tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(filePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            extractedText = page.GetText();
                        }
                    }
                }
                return Ok(new { text = extractedText });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing OCR in ExtractTextFromImage");
                return StatusCode(500, new { message = "An error occurred while processing the image." });
            }
            finally
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch
                    {
                        // Ignore cleanup error
                    }
                }
            }
        }
    }
}
