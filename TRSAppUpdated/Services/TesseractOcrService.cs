using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Tesseract;
using TRSAppUpdated.Interfaces;

namespace TRSAppUpdated.Services
{
    public class TesseractOcrService : IOcrService, IDisposable
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<TesseractOcrService> _logger;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private TesseractEngine? _engine;
        private bool _disposed;

        public TesseractOcrService(IWebHostEnvironment webHostEnvironment, ILogger<TesseractOcrService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        private TesseractEngine GetOrCreateEngine()
        {
            if (_engine != null)
                return _engine;

            string tessdataPath = ResolveTessdataPath();
            _logger.LogInformation("Initializing Tesseract OCR engine with tessdata path: {Path}", tessdataPath);

            _engine = new TesseractEngine(tessdataPath, "eng", EngineMode.Default);
            return _engine;
        }

        private string ResolveTessdataPath()
        {
            // 1. Environment variable if configured in Railway / Docker
            var envPath = Environment.GetEnvironmentVariable("TESSDATA_PREFIX");
            if (!string.IsNullOrEmpty(envPath) && Directory.Exists(envPath))
            {
                return envPath;
            }

            // 2. WebRootPath assets/tessdata
            var localAssetPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "tessdata");
            if (Directory.Exists(localAssetPath))
            {
                return localAssetPath;
            }

            // 3. Fallback standard Linux paths
            string[] linuxPaths =
            {
                "/usr/share/tesseract-ocr/5/tessdata",
                "/usr/share/tesseract-ocr/4.00/tessdata",
                "/usr/share/tessdata"
            };

            foreach (var path in linuxPaths)
            {
                if (Directory.Exists(path))
                {
                    return path;
                }
            }

            return localAssetPath;
        }

        public async Task<string> ExtractTextFromImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            // Preprocess image with ImageSharp (resize large mobile photos and convert to grayscale for faster & more accurate OCR)
            byte[] processedImageBytes;
            using (var uploadStream = file.OpenReadStream())
            {
                using var image = await Image.LoadAsync(uploadStream);

                image.Mutate(ctx =>
                {
                    if (image.Width > 1800 || image.Height > 1800)
                    {
                        ctx.Resize(new ResizeOptions
                        {
                            Size = new Size(1800, 1800),
                            Mode = ResizeMode.Max
                        });
                    }
                    ctx.Grayscale();
                });

                using var outputMs = new MemoryStream();
                await image.SaveAsync(outputMs, new PngEncoder());
                processedImageBytes = outputMs.ToArray();
            }

            // Acquire lock to guarantee thread-safe use of the native Tesseract engine
            await _semaphore.WaitAsync();
            try
            {
                var engine = GetOrCreateEngine();
                using var pix = Pix.LoadFromMemory(processedImageBytes);
                using var page = engine.Process(pix);
                return page.GetText()?.Trim() ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image with Tesseract OCR");
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _engine?.Dispose();
            _semaphore.Dispose();
        }
    }
}

