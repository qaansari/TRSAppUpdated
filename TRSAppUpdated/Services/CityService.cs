using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class CityService : ICityService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CityService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "City";

        public CityService(AppDbContext context, ILogger<CityService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<CityVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.Cities, () =>
            {
                try
                {
                    var result = _context.Cities.Where(x => !x.IsDeleted)
                                 .Select(x => new CityVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                     CountryID = x.CountryID,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<CityVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<CityVM>();
            }, TimeSpan.FromHours(4));
        }
    }
}
