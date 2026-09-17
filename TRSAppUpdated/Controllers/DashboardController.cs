using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEventService _eventService;
        private readonly IShowService _showService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<DashboardController> _logger;
        private readonly string _Title = "Dashboard";

        public DashboardController(AppDbContext context, ILogger<DashboardController> logger,
            IEventService eventService, IShowService showService, ICacheService cacheService)
        {
            _context = context;
            _eventService = eventService;
            _showService = showService;
            _cacheService = cacheService;
            _logger = logger;
        }

        [Authorize(Roles = "Super Admin,Admin,Moderator,User")]
        public async Task<IActionResult> Index(int EventID = 0, int ShowID = 0)
        {
            var _object = new DashboardVM();
            try
            {
                ViewData["EventList"] = _eventService.GetAll();
                ViewData["ShowList"] = _showService.GetAll();

                if (EventID == 0)
                {
                    EventID = await _context.Events
                        .Where(x => !x.IsDeleted && x.EventStatusID == 1)
                        .OrderByDescending(x => x.ID)
                        .Select(x => x.ID)
                        .FirstOrDefaultAsync();
                }
                ViewBag.EventID = EventID;
                ViewBag.ShowID = ShowID;

                if (EventID != 0)
                {
                    var _event = await _context.Events
                        .Where(x => !x.IsDeleted && x.ID == EventID)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (_event != null)
                    {
                        _object.EventTitle = _event.Title;
                        _object.EventImagePath = _event.ImagePath;
                        if (ShowID == 0)
                        {
                            var showIDList = await _context.Shows
                                .Where(x => !x.IsDeleted && x.EventID == EventID)
                                .Select(x => x.ID)
                                .ToListAsync();

                            _object.Expense = await _context.ShowExpenses
                                .Where(x => !x.IsDeleted && showIDList.Contains(x.ShowID))
                                .SumAsync(x => x.Amount);

                            _object.Revenue = await _context.Reservations
                                .Where(x => !x.IsDeleted 
                                    && x.IsApproved.HasValue 
                                    && x.IsApproved.Value 
                                    && showIDList.Contains(x.ShowID))
                                .SumAsync(x => x.AmountPaid);
                        }
                        else
                        {
                            _object.Expense = await _context.ShowExpenses
                                .Where(x => !x.IsDeleted && x.ShowID == ShowID)
                                .SumAsync(x => x.Amount);

                            _object.Revenue = await _context.Reservations
                                .Where(x => !x.IsDeleted 
                                    && x.IsApproved.HasValue
                                    && x.IsApproved.Value
                                    && x.ShowID == ShowID)
                                .SumAsync(x => x.AmountPaid);
                        }
                        _object.Profit = _object.Revenue - _object.Expense;
                    }
                }

                // Retrieve cached summary counters (TTL: 5 minutes)
                var counts = await _cacheService.GetOrCreateAsync(CacheKeys.DashboardMetrics, async () =>
                {
                    var currentDate = DateTime.UtcNow.AddHours(5).Date;
                    var nextMonthDate = currentDate.AddDays(30);

                    var venues = await _context.Venues.CountAsync(x => !x.IsDeleted);
                    var events = await _context.Events.CountAsync(x => !x.IsDeleted
                        && x.StartDate >= currentDate
                        && x.StartDate <= nextMonthDate);
                    var users = await _context.Users.CountAsync(x => !x.IsDeleted
                        && x.IsVerified
                        && x.IsActive
                        && x.RoleID == 4);

                    return new[] { venues, events, users };
                }, TimeSpan.FromMinutes(5));

                if (counts != null && counts.Length == 3)
                {
                    _object.TotalVenues = counts[0];
                    _object.TotalEvents = counts[1];
                    _object.TotalUsers = counts[2];
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index in {_Title} Controller due to {ex}");
            }
            return View(_object);
        }
    }
}
