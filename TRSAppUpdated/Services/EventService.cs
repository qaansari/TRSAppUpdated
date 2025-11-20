using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Linq;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EventService> _logger;
        private readonly string _Title = "Event";
        public EventService(AppDbContext context, ILogger<EventService> logger)
        {
            _context = context;
            _logger = logger;
        }
        //Get All
        public List<EventVM> GetAll()
        {
            try
            {
                var _result = _context.Events.Where(x => !x.IsDeleted)
                             .Select(x => new EventVM()
                             {
                                 ID = x.ID,
                                 Title = x.Title,
                             })
                             .AsNoTracking()
                             .OrderBy(x => x.Title)
                             .ToList();
                return _result ?? new List<EventVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
            }
            return new List<EventVM>();
        }
        //Get All Paginated
        public async Task<(List<EventVM> _result, int totalRecords)> GetAllPaginated(int _pageNo = 1, int _pageSize = 10, string _searchedTerm = "", int _CountryID = 0)
        {
            var _result = new List<EventVM>();
            int totalRecords = 0;
            try
            {
                // For AJAX calls
                var _query = _context.Events
                    .Where(x => !x.IsDeleted)
                           .Select(x => new EventVM()
                           {
                               ID = x.ID,
                               Title = x.Title,
                               CountryID = x.Auditorium.Venue.City.CountryID,
                               CountryTitle = x.Auditorium.Venue.City.Country.Title,
                               CityTitle = x.Auditorium.Venue.City.Title,
                               VenueTitle = x.Auditorium.Venue.Title,
                               AuditoriumTitle = x.Auditorium.Title,
                               EventStatusTitle = x.EventStatus.Title,
                               ImagePath = x.ImagePath
                           })
                           .AsNoTracking()
                           .AsQueryable();
                if (_CountryID != 0)
                {
                    _query = _query.Where(x => x.CountryID == _CountryID);
                }
                if (!string.IsNullOrEmpty(_searchedTerm))
                {
                    _query = _query
                        .Where(x => x.Title.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                        || x.CityTitle.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                        || x.VenueTitle.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                        || x.AuditoriumTitle.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                        || x.EventStatusTitle.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower()));
                }
                int _skip = (_pageNo - 1) * _pageSize;
                totalRecords = await _query.CountAsync();
                _query = _query.Skip(_skip).Take(_pageSize);
                _result = await _query.OrderByDescending(x => x.ID).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetAllPaginated in {_Title} Service due to {ex}");
            }
            return (_result, totalRecords);
        }
        //Get Details By ID
        public async Task<EventVM> GetDetailsByID(int ID)
        {
            // For AJAX calls
            try
            {
                var _object = await _context.Events
     .Where(x => !x.IsDeleted && x.ID == ID)
     .Select(x => new EventVM()
     {
         ID = x.ID,
         Title = x.Title,
         StartDate = x.StartDate,
         EndDate = x.EndDate,
         Description = x.Description,
         CountryID = x.Auditorium.Venue.City.CountryID,
         CountryTitle = x.Auditorium.Venue.City.Country.Title,
         CityTitle = x.Auditorium.Venue.City.Title,
         VenueTitle = x.Auditorium.Venue.Title,
         AuditoriumTitle = x.Auditorium.Title,
         EventStatusTitle = x.EventStatus.Title,
         ShowsList = x.Shows
             .Where(s => !s.IsDeleted)
             .OrderBy(x => x.Date)
             .Select(s => new ShowVM()
             {
                 ID = s.ID,
                 Date = s.Date,
                 Time = s.Time,
                 IsEnded = s.IsEnded,
             })
             .ToList(),
         EventCategoriesList = x.EventCategories
              .Select(ec => ec.Category.Title)
             .ToList(),
         ReservationList = x.Shows
             .SelectMany(s => s.Reservations)
             .Select(r => new ReservationVM()
             {
                 ID = r.ID,
                 TicketNumber = r.TicketNumber,
                 ShowDateTime = r.Show.Date.ToString("dddd, dd MMMM yyyy") + " "
                 + DateTime.Today.Add(r.Show.Time).ToString("hh:mm tt"),
                 CustomerName = r.CustomerName,
                 CustomerPhone = r.CustomerPhone,
                 SalePrice = r.SalePrice,
                 CreatedByUserID = r.CreatedByUserID,
                 AmountPaid = r.AmountPaid,
                 IsApproved = r.IsApproved,
                 ReservationStatusTitle = r.ReservationStatus.Title,
                 ApprovedByUserName = r.ActionByUser.FullName,
                 CreatedByUserName = r.CreatedByUser.FullName,
             })
             .ToList(),
         ImagePath = x.ImagePath
     })
            .AsNoTracking()
            .FirstOrDefaultAsync();
                return _object ?? new EventVM();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetDetailsByID in {_Title} Service due to {ex}");
            }
            return new EventVM();
        }
    }
}
