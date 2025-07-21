using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using Syncfusion.EJ2.Layouts;
using System.Diagnostics;
using System.Security.Claims;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly string _Title = "Home";
        private readonly ICountryService _countryService;
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(AppDbContext context, ICountryService countryService,
            ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _countryService = countryService;
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
            try
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
                        TempData["Message"] = "1,A verification email has been sent to your email";
                        string _verificationLink = AppHelper._appUrl + "User/VerifyEmail?token=" + user.UserToken;
                        AppHelper.SendVerificationEmail(user.Email, "verify email", _verificationLink);
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index (POST) in {_Title} Controller due to {ex}");
            }
            return View(Model);
        }
        //Sign up (GET)
        public IActionResult Signup()
        {
            ViewData["CountryList"] = _countryService.GetAll();
            return View(new UserVM() { RoleID = 4 });
        }
        //Sign up (POST)
        [HttpPost]
        public async Task<IActionResult> Signup(UserVM Model)
        {
            try
            {
                ViewData["CountryList"] = _countryService.GetAll();
                if (ModelState.IsValid)
                {
                    var _item = new User();
                    _item.FirstName = Model.FirstName.Trim();
                    _item.LastName = Model.LastName.Trim();
                    _item.ContactNumber = Model.ContactNumber.Trim();
                    _item.RoleID = Model.RoleID;
                    _item.Email = Model.Email.Trim();
                    _item.Password = Model.Password.Trim().HashWithSalt();
                    _item.IsActive = Model.IsActive;
                    _item.IsVerified = Model.IsVerified;
                    _item.UserToken = Guid.NewGuid().ToString();
                    _item.IsTokenExpired = false;
                    _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                    _context.Users.Add(_item);
                    await _context.SaveChangesAsync();
                    if (_item.ID != 0)
                    {
                        TempData["Message"] = "1,A verification email has been sent to your email";
                        string _verificationLink = AppHelper._appUrl + "User/VerifyEmail?token=" + _item.UserToken;
                        AppHelper.SendVerificationEmail(_item.Email, "verify email", _verificationLink);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Signup (POST) in {_Title} Controller due to {ex}");
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
        public IActionResult Verify(bool IsVerified = false)
        {
            ViewBag.IsVerified = IsVerified;
            return View();
        }
    }
}
