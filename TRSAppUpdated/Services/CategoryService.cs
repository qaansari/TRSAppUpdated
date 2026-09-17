using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Category";

        public CategoryService(AppDbContext context, ILogger<CategoryService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<CategoryVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.Categories, () =>
            {
                try
                {
                    var result = _context.Categories.Where(x => !x.IsDeleted)
                                 .Select(x => new CategoryVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<CategoryVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<CategoryVM>();
            }, TimeSpan.FromHours(4));
        }
    }
}
