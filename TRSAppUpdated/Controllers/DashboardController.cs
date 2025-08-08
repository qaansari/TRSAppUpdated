using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DashboardController> _logger;
        private readonly string _Title = "Dashboard";
        public DashboardController(AppDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [Authorize(Roles = "Super Admin,Admin,Moderator,User")]
        public IActionResult Index()
        {
            var _object = new DashboardVM();
            try
            {
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
