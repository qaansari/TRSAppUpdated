using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class ReservationStatusService : IReservationStatusService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReservationStatusService> _logger;
        private readonly string _Title = "Reservation Status";
        public ReservationStatusService(AppDbContext context, ILogger<ReservationStatusService> logger)
        {
            _context = context;
            _logger = logger;
        }
        //Get All
        public List<ReservationStatusVM> GetAll()
        {
            try
            {
                var _result = _context.ReservationStatuses.Where(x => !x.IsDeleted)
                             .Select(x => new ReservationStatusVM()
                             {
                                 ID = x.ID,
                                 Title = x.Title,
                             })
                             .AsNoTracking()
                             .OrderBy(x => x.Title)
                             .ToList();
                return _result ?? new List<ReservationStatusVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
            }
            return new List<ReservationStatusVM>();
        }
    }
}
