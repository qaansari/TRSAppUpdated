using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<DashboardController> _logger;
        private readonly string _Title = "Dashboard";
        public DashboardController(AppDbContext context, ILogger<DashboardController> logger,IEventService eventService,IShowService showService)
        {
            _context = context;
            _eventService = eventService;
            _showService = showService;
            _logger = logger;
        }
        [Authorize(Roles = "Super Admin,Admin,Moderator,User")]
        public IActionResult Index(int EventID = 0, int ShowID = 0)
        {
            var _object = new DashboardVM();
            try
            {
                ViewData["EventList"] = _eventService.GetAll();
                ViewData["ShowList"] = _showService.GetAll();
                if (EventID == 0)
                {
                    EventID = _context.Events.Where(x => !x.IsDeleted && x.EventStatusID==1).OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault();
                }
                ViewBag.EventID = EventID;
                ViewBag.ShowID = ShowID;
                var _event = _context.Events.Where(x => !x.IsDeleted && x.ID == EventID).FirstOrDefault();
                if (_event != null)
                {
                    _object.EventTitle = _event.Title;
                    _object.EventImagePath = _event.ImagePath;
                    if (ShowID == 0)
                    {
                        List<int> showIDList = _context.Shows.Where(x => x.EventID == EventID).Select(x => x.ID).ToList();
                        _object.Expense = _context.ShowExpenses.Where(x => !x.IsDeleted && showIDList.Contains(x.ShowID)).Sum(x => x.Amount);
                        _object.Revenue = _context.Reservations.Where(x => !x.IsDeleted 
                        && x.IsApproved.HasValue 
                        && x.IsApproved.Value==true 
                        && showIDList.Contains(x.ShowID)).Sum(x => x.AmountPaid);
                    }
                    else
                    {
                        _object.Expense = _context.ShowExpenses.Where(x => !x.IsDeleted && x.ShowID==ShowID).Sum(x => x.Amount);
                        _object.Revenue = _context.Reservations.Where(x => !x.IsDeleted && x.IsApproved.HasValue
                        && x.IsApproved.Value == true
                        && x.ShowID == ShowID).Sum(x => x.AmountPaid);
                    }
                    _object.Profit = _object.Revenue - _object.Expense;
                }
                var _currentDate = DateTime.UtcNow.AddHours(5);
                var _nextMonthDate = DateTime.UtcNow.AddHours(5).AddDays(30);
                _object.TotalVenues = _context.Venues.Where(x => !x.IsDeleted).Count();
                _object.TotalEvents = _context.Events.Where(x => !x.IsDeleted
                && x.StartDate.Date >= _currentDate.Date
                && x.StartDate.Date <= _nextMonthDate.Date).Count();
                _object.TotalUsers = _context.Users.Where(x => !x.IsDeleted
                && x.IsVerified
                && x.IsActive
                && x.RoleID == 4).Count();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index in {_Title} Controller due to {ex}");
            }
            return View(_object);
        }
    }
}
