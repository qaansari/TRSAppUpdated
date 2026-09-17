using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class EventStatusService : IEventStatusService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EventStatusService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Event Status";

        public EventStatusService(AppDbContext context, ILogger<EventStatusService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<EventStatusVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.EventStatuses, () =>
            {
                try
                {
                    var result = _context.EventStatuses.Where(x => !x.IsDeleted)
                                 .Select(x => new EventStatusVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<EventStatusVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<EventStatusVM>();
            }, TimeSpan.FromHours(4));
        }
    }
}
