using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EventService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Event";

        public EventService(AppDbContext context, ILogger<EventService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<EventVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.EventsList, () =>
            {
                try
                {
                    var result = _context.Events.Where(x => !x.IsDeleted)
                                 .Select(x => new EventVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<EventVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<EventVM>();
            }, TimeSpan.FromHours(1));
        }

        //Get All Paginated - Fixed OrderBy before Skip/Take
        public async Task<(List<EventVM> _result, int totalRecords)> GetAllPaginated(int _pageNo = 1, int _pageSize = 10, string _searchedTerm = "", int _CountryID = 0)
        {
            var result = new List<EventVM>();
            int totalRecords = 0;
            try
            {
                var query = _context.Events
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
                    query = query.Where(x => x.CountryID == _CountryID);
                }

                if (!string.IsNullOrEmpty(_searchedTerm))
                {
                    string term = _searchedTerm.Trim().ToLower();
                    query = query.Where(x => x.Title.ToLower().Contains(term)
                        || x.CityTitle.ToLower().Contains(term)
                        || x.VenueTitle.ToLower().Contains(term)
                        || x.AuditoriumTitle.ToLower().Contains(term)
                        || x.EventStatusTitle.ToLower().Contains(term));
                }

                totalRecords = await query.CountAsync();
                int skip = (_pageNo - 1) * _pageSize;

                // Crucial fix: OrderBy MUST precede Skip and Take
                result = await query.OrderByDescending(x => x.ID)
                                    .Skip(skip)
                                    .Take(_pageSize)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetAllPaginated in {_Title} Service due to {ex}");
            }
            return (result, totalRecords);
        }

        //Get Details By ID - Fixed in-memory date formatting
        public async Task<EventVM> GetDetailsByID(int ID)
        {
            try
            {
                var ev = await _context.Events
                    .Where(x => !x.IsDeleted && x.ID == ID)
                    .Include(x => x.Auditorium).ThenInclude(a => a.Venue).ThenInclude(v => v.City).ThenInclude(c => c.Country)
                    .Include(x => x.EventStatus)
                    .Include(x => x.EventCategories).ThenInclude(ec => ec.Category)
                    .Include(x => x.Shows.Where(s => !s.IsDeleted))
                        .ThenInclude(s => s.Reservations.Where(r => !r.IsDeleted))
                            .ThenInclude(r => r.ReservationStatus)
                    .Include(x => x.Shows.Where(s => !s.IsDeleted))
                        .ThenInclude(s => s.Reservations.Where(r => !r.IsDeleted))
                            .ThenInclude(r => r.ActionByUser)
                    .Include(x => x.Shows.Where(s => !s.IsDeleted))
                        .ThenInclude(s => s.Reservations.Where(r => !r.IsDeleted))
                            .ThenInclude(r => r.CreatedByUser)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (ev == null)
                {
                    return new EventVM();
                }

                var vm = new EventVM
                {
                    ID = ev.ID,
                    Title = ev.Title,
                    StartDate = ev.StartDate,
                    EndDate = ev.EndDate,
                    Description = ev.Description,
                    CountryID = ev.Auditorium?.Venue?.City?.CountryID ?? 0,
                    CountryTitle = ev.Auditorium?.Venue?.City?.Country?.Title ?? string.Empty,
                    CityTitle = ev.Auditorium?.Venue?.City?.Title ?? string.Empty,
                    VenueTitle = ev.Auditorium?.Venue?.Title ?? string.Empty,
                    AuditoriumTitle = ev.Auditorium?.Title ?? string.Empty,
                    EventStatusTitle = ev.EventStatus?.Title ?? string.Empty,
                    ImagePath = ev.ImagePath,
                    ShowsList = ev.Shows
                        .Where(s => !s.IsDeleted)
                        .OrderBy(s => s.Date)
                        .Select(s => new ShowVM
                        {
                            ID = s.ID,
                            Date = s.Date,
                            Time = s.Time,
                            IsEnded = s.IsEnded
                        }).ToList(),
                    EventCategoriesList = ev.EventCategories
                        .Select(ec => ec.Category?.Title ?? string.Empty)
                        .ToList()
                };

                var reservations = new List<ReservationVM>();
                foreach (var show in ev.Shows.Where(s => !s.IsDeleted))
                {
                    string showDateStr = show.Date.ToString("dddd, dd MMMM yyyy") + " "
                        + DateTime.Today.Add(show.Time).ToString("hh:mm tt");

                    foreach (var r in show.Reservations.Where(r => !r.IsDeleted))
                    {
                        reservations.Add(new ReservationVM
                        {
                            ID = r.ID,
                            TicketNumber = r.TicketNumber,
                            ShowDateTime = showDateStr,
                            CustomerName = r.CustomerName,
                            CustomerPhone = r.CustomerPhone,
                            AmountRemaining = r.AmountRemaining,
                            SalePrice = r.SalePrice,
                            CreatedByUserID = r.CreatedByUserID,
                            AmountPaid = r.AmountPaid,
                            IsApproved = r.IsApproved,
                            ReservationStatusTitle = r.ReservationStatus?.Title ?? string.Empty,
                            ApprovedByUserName = r.ActionByUser?.FullName ?? string.Empty,
                            CreatedByUserName = r.CreatedByUser?.FullName ?? string.Empty,
                        });
                    }
                }
                vm.ReservationList = reservations;
                return vm;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetDetailsByID in {_Title} Service due to {ex}");
            }
            return new EventVM();
        }
    }
}
