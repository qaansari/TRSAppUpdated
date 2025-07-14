using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Tesseract;

namespace TRSAppUpdated.Controllers
{
    public class ImageProcessController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageProcessController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost]
        public async Task<IActionResult> ExtractTextFromImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            try
            {
                string extractedText;
                // Save the file temporarily
                var filePath = Path.GetTempFileName();
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
                // Delete the temporary file
                System.IO.File.Delete(filePath);
                return Ok(new { text = extractedText });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
