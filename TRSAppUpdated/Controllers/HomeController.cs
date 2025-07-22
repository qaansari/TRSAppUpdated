using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
                            UserToken = x.UserToken,
                            CountryID = x.CountryID,
                            TokenExpiryTime = x.TokenExpiryTime,
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
                        if (!string.IsNullOrEmpty(user.UserToken) && user.TokenExpiryTime.HasValue)
                        {
                            if (DateTime.UtcNow.AddHours(5) > user.TokenExpiryTime.Value)
                            {
                                user.UserToken = Guid.NewGuid().ToString();
                            }
                        }
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
             ExpiresUtc = Model.IsPersistent ? DateTime.UtcNow.AddHours(5).AddDays(1) : DateTime.UtcNow.AddHours(5).AddHours(1) // Customize as needed
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
                    var isEmailExists = _context.Users.Where(x => !x.IsDeleted
                           && x.Email.Trim().ToLower() == Model.Email.Trim().ToLower())
                        .Select(x => x.Email)
                       .Any();
                    var isMobileNoExists = _context.Users.Where(x => !x.IsDeleted
                          && x.ContactNumber.Trim().ToLower() == Model.ContactNumber.Trim().ToLower())
                       .Select(x => x.ContactNumber)
                      .Any();
                    if (isEmailExists)
                    {
                        TempData["Message"] = $"0,Email Already Taken";
                        return View(Model);
                    }
                    if (isMobileNoExists)
                    {
                        TempData["Message"] = $"0,Contact Number Already Taken";
                        return View(Model);
                    }
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
                    _item.TokenExpiryTime = DateTime.UtcNow.AddHours(5).AddMinutes(10);
                    _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                    _context.Users.Add(_item);
                    await _context.SaveChangesAsync();
                    if (_item.ID != 0)
                    {
                        TempData["Message"] = "1,A verification email has been sent to your email";
                        string _verificationLink = AppHelper._appUrl + "Home/VerifyEmail?token=" + _item.UserToken;
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
        public async Task<IActionResult> VerifyEmail(string token)
        {
            try
            {
                var _user = _context.Users.Where(x => !x.IsDeleted && x.UserToken == token).FirstOrDefault();
                if (_user != null)
                {
                    _user.IsVerified = true;
                    _user.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                    _context.Entry(_user).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"1,{_Title} Successfully Verified";
                }
                return RedirectToAction("Verify", "Home", new { IsVerified = _user?.IsVerified });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at VerifyEmail in {_Title} Controller due to {ex}");
            }
            return RedirectToAction("Index");
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
        //Forget Password (GET)
        public IActionResult ForgetPassword()
        {
            return View(new ResetPasswordVM());
        }
        //Reset Password (POST)
        public IActionResult ResetPassword(ResetPasswordVM Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Message"] = "1,A verification email has been sent to your email";
                    //string _verificationLink = AppHelper._appUrl + "User/VerifyEmail?token=" + _item.UserToken;
                    //AppHelper.SendVerificationEmail(Model.Email, "verify email", _verificationLink);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at ResetPassword (POST) in {_Title} Controller due to {ex}");
            }
            return View(Model);
        }
    }
}
