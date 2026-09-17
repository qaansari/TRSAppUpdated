using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class VenueService : IVenueService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VenueService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Venue";

        public VenueService(AppDbContext context, ILogger<VenueService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<VenueVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.Venues, () =>
            {
                try
                {
                    var result = _context.Venues.Where(x => !x.IsDeleted)
                                 .Select(x => new VenueVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                     CityID = x.CityID,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<VenueVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<VenueVM>();
            }, TimeSpan.FromHours(4));
        }
    }
}
