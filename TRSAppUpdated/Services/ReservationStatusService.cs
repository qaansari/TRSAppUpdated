using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class ReservationStatusService : IReservationStatusService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReservationStatusService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Reservation Status";

        public ReservationStatusService(AppDbContext context, ILogger<ReservationStatusService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<ReservationStatusVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.ReservationStatuses, () =>
            {
                try
                {
                    var result = _context.ReservationStatuses.Where(x => !x.IsDeleted)
                                 .Select(x => new ReservationStatusVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<ReservationStatusVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<ReservationStatusVM>();
            }, TimeSpan.FromHours(4));
        }
    }
}
