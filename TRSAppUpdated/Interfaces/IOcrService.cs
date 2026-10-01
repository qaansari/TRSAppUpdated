using Microsoft.AspNetCore.Http;

namespace TRSAppUpdated.Interfaces
{
    public interface IOcrService
    {
        Task<string> ExtractTextFromImageAsync(IFormFile file);
    }
}

