using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class CountryService : ICountryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CountryService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Country";

        public CountryService(AppDbContext context, ILogger<CountryService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<CountryVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.Countries, () =>
            {
                try
                {
                    var result = _context.Countries.Where(x => !x.IsDeleted)
                                 .Select(x => new CountryVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<CountryVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<CountryVM>();
            }, TimeSpan.FromHours(4));
        }
    }
}
