using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ReservationController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly string _Title = "Event";
        private readonly int _UserID;
        public ReservationController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment, ILogger<ReservationController> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
        }
        public IActionResult AddOrEdit(int ID = 0, int EventID = 0)
        {
            if()
            return View();
        }
    }
}
