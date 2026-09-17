using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuditoriumService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Auditorium";

        public AuditoriumService(AppDbContext context, ILogger<AuditoriumService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<AuditoriumVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.Auditoriums, () =>
            {
                try
                {
                    var result = _context.Auditoriums.Where(x => !x.IsDeleted)
                                 .Select(x => new AuditoriumVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                     VenueID = x.VenueID,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<AuditoriumVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<AuditoriumVM>();
            }, TimeSpan.FromHours(4));
        }
    }
}
