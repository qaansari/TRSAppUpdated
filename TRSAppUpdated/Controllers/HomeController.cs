using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(AppDbContext context, ILogger<HomeController> logger, 
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        //Index (Login) (GET)
        public IActionResult Index()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View(new LoginVM());
        }
        //Index (Login) (POST)
        [HttpPost]
        public async Task<IActionResult> Index(LoginVM Model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = Model.Password.HashWithSalt();
                var user = _context.Users.Include(x => x.Role)
                    .Where(x => !x.IsDeleted
                    && x.Email == Model.Email
                    && x.Password == hashedPassword)
                    .Select(x => new UserVM()
                    {
                        ID = x.ID,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email,
                        ContactNumber = x.ContactNumber,
                        ImagePath = x.ImagePath,
                        RoleID = x.RoleID,
                        RoleTitle = x.Role.Title,
                        IsActive = x.IsActive,
                        IsVerified = x.IsVerified
                    })
                    .FirstOrDefault();
                if (user == null)
                {
                    TempData["Message"] = "0,Invalid Username or Password";
                    return View(Model);
                }
                else if (!user.IsVerified)
                {
                    TempData["Message"] = "0,Please verify your account first";
                    return View(Model);
                }
                else if (!user.IsActive)
                {
                    TempData["Message"] = "0,Please ask the admin to unblock your account";
                    return View(Model);
                }
                var claims = new List<Claim>() {
            new Claim("UserID",user.ID.ToString()),
            new Claim("Username",user.FullName.Trim()),
            new Claim("RoleID",user.RoleID.ToString()),
            new Claim(ClaimTypes.Role,!string.IsNullOrEmpty(user.RoleTitle)? user.RoleTitle:""),
            new Claim("Image",!string.IsNullOrEmpty(user.ImagePath)?user.ImagePath.Trim():string.Empty),
            };
                var claimsIndentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIndentity);
                await _httpContextAccessor.HttpContext.SignInAsync(
     CookieAuthenticationDefaults.AuthenticationScheme,
     claimsPrincipal,
     new AuthenticationProperties
     {
         IsPersistent = Model.IsPersistent,
         ExpiresUtc = Model.IsPersistent ? DateTime.UtcNow.AddHours(5).AddDays(14) : DateTime.UtcNow.AddHours(5).AddHours(1) // Customize as needed
     });
                TempData["Message"] = $"1,Welcome {user.FullName}";
                return RedirectToAction("Index", "Dashboard");
            }
            return View(Model);
        }
        //Logout
        public async Task<IActionResult> Logout()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        //Privacy
        public IActionResult Privacy()
        {
            return View();
        }
        //Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
