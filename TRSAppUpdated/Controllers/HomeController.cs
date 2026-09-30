using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public HomeController(AppDbContext context, ICountryService countryService,
            ILogger<HomeController> logger, IUserService userService,
            IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
        {
            _context = context;
            _countryService = countryService;
            _logger = logger;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
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
        [EnableRateLimiting("auth-limit")]
        public async Task<IActionResult> Index(LoginVM Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var inputEmail = Model.Email?.Trim() ?? string.Empty;
                    var hashedPassword = Model.Password.HashWithSalt();
                    var user = _context.Users
                        .Where(x => !x.IsDeleted
                        && x.Email == inputEmail
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
                        var userEntity = await _context.Users.FirstOrDefaultAsync(x => !x.IsDeleted && x.ID == user.ID);
                        if (userEntity != null)
                        {
                            if (string.IsNullOrEmpty(userEntity.UserToken) || !userEntity.TokenExpiryTime.HasValue || DateTime.UtcNow.AddHours(5) > userEntity.TokenExpiryTime.Value)
                            {
                                userEntity.UserToken = Guid.NewGuid().ToString();
                                userEntity.TokenExpiryTime = DateTime.UtcNow.AddHours(5).AddMinutes(10);
                                userEntity.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                                _context.Entry(userEntity).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                                user.UserToken = userEntity.UserToken;
                            }
                        }
                        TempData["Message"] = "1,A verification email has been sent to your email";
                        string _verificationLink = AppHelper._appUrl + "Home/VerifyEmail?token=" + user.UserToken;
                        await AppHelper.SendVerificationEmailAsync(user.Email, "verify email", _verificationLink, _configuration);
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
            new Claim("ContactNumber",user.ContactNumber.Trim()),
            new Claim("RoleID",user.RoleID.ToString()),
            new Claim(ClaimTypes.Role,!string.IsNullOrEmpty(user.RoleTitle)? user.RoleTitle:""),
            new Claim("Image",!string.IsNullOrEmpty(user.ImagePath)?user.ImagePath.Trim():string.Empty),
            };
                    var claimsIndentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIndentity);
                    var authProperties = new AuthenticationProperties
                    {
                        // IMPORTANT: Remove AddHours(5) completely
                        ExpiresUtc = Model.IsPersistent
         ? DateTimeOffset.UtcNow.AddDays(1)  // 24 hours
         : DateTimeOffset.UtcNow.AddHours(1), // 1 hour
                                              // Add these for server stability
                        AllowRefresh = true,
                        IssuedUtc = DateTimeOffset.UtcNow,
                        // This helps with server time sync
                        IsPersistent = Model.IsPersistent
                    };
                    await _httpContextAccessor.HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        claimsPrincipal,
                        authProperties);

                    TempData["Message"] = $"1,Welcome {user.FullName}";
                    var cookie = _httpContextAccessor.HttpContext.Response.Headers["Set-Cookie"];
                    _logger.LogInformation($"Auth Cookie Set: {cookie}");
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index (POST) in {_Title} Controller due to {ex}");
                TempData["Message"] = "0,An error occurred while logging in. Please check your credentials.";
            }
            return View(Model);
        }
        [Authorize]
        public IActionResult KeepAlive()
        {
            // This will refresh the cookie due to sliding expiration
            return Json(new { success = true });
        }
        //Sign up (GET)
        public IActionResult Signup()
        {
            ViewData["CountryList"] = _countryService.GetAll();
            return View(new UserVM() { RoleID = 4 });
        }
        //Sign up (POST)
        [HttpPost]
        [EnableRateLimiting("auth-limit")]
        public async Task<IActionResult> Signup(UserVM Model)
        {
            try
            {
                ViewData["CountryList"] = _countryService.GetAll();

                // Security: Public registration MUST always assign the standard User role (RoleID = 4)
                Model.RoleID = 4;

                if (string.IsNullOrWhiteSpace(Model.Password))
                {
                    ModelState.AddModelError("Password", "Password is required");
                }

                if (Model.ImageFile != null && !AppHelper.IsValidImageFile(Model.ImageFile, out var imgError))
                {
                    ModelState.AddModelError("ImageFile", imgError ?? "Invalid image file");
                }

                if (ModelState.IsValid)
                {
                    if (Model.CountryID == 0 || Model.CountryID == null)
                    {
                        ModelState.AddModelError(nameof(Model.CountryID), "Please select a country");
                        return View(Model);
                    }
                    var isEmailExists = await _context.Users.Where(x => !x.IsDeleted
                           && x.Email.Trim().ToLower() == Model.Email.Trim().ToLower())
                        .Select(x => x.Email)
                       .AnyAsync();
                    var isMobileNoExists = await _context.Users.Where(x => !x.IsDeleted
                          && x.ContactNumber.Trim().ToLower() == Model.ContactNumber.Trim().ToLower())
                       .Select(x => x.ContactNumber)
                      .AnyAsync();
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
                    var now = AppHelper.NowPakistan;
                    var _item = new User();
                    _item.FirstName = Model.FirstName.Trim();
                    _item.LastName = Model.LastName.Trim();
                    _item.ContactNumber = Model.ContactNumber.Trim();
                    _item.RoleID = 4; // Hardened against parameter tampering
                    _item.CountryID = Model.CountryID;
                    _item.Email = Model.Email.Trim();
                    _item.Password = Model.Password.Trim().HashWithSalt();
                    _item.IsActive = Model.IsActive;
                    _item.IsVerified = Model.IsVerified;
                    _item.UserToken = Guid.NewGuid().ToString("N");
                    _item.TokenExpiryTime = now.AddMinutes(15);
                    _item.CreatedDateTime = now;
                    _context.Users.Add(_item);
                    await _context.SaveChangesAsync();
                    if (_item != null && Model.ImageFile != null)
                    {
                        var fileName = await AppHelper.SaveUploadedImageAsync(Model.ImageFile, _webHostEnvironment.WebRootPath, "users", "user");
                        _item.ImagePath = fileName;
                        _item.UpdatedDateTime = now;
                        _context.ChangeTracker.Clear();
                        _context.Entry(_item).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    if (_item.ID != 0)
                    {
                        TempData["Message"] = "1,A verification email has been sent to your email";
                        string _verificationLink = AppHelper._appUrl + "Home/VerifyEmail?token=" + _item.UserToken;
                        await AppHelper.SendVerificationEmailAsync(_item.Email, "verify email", _verificationLink, _configuration);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Signup (POST) in {_Title} Controller due to {ex}");
            }
            return View(Model);
        }
        //Verify Email
        public async Task<IActionResult> VerifyEmail(string token)
        {
            try
            {
                var _user = _userService.GetUserByToken(token);
                if (_user != null)
                {
                    _user.IsVerified = true;
                    _user.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                    _context.Entry(_user).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"1,User Successfully Verified";
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
            return View(new ForgetPasswordVM());
        }
        //Forget Password (POST)
        [HttpPost]
        [EnableRateLimiting("auth-limit")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _user = _userService.GetUserByEmail(Model.Email);
                    if (_user == null)
                    {
                        TempData["Message"] = "0,No user found for given email";
                        return View(Model);
                    }
                    var now = AppHelper.NowPakistan;
                    if (string.IsNullOrEmpty(_user.UserToken) || !_user.TokenExpiryTime.HasValue || now > _user.TokenExpiryTime.Value)
                    {
                        _user.UserToken = Guid.NewGuid().ToString("N");
                        _user.TokenExpiryTime = now.AddMinutes(15);
                        _context.Entry(_user).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    TempData["Message"] = "1,A reset password link has been sent to your email";
                    string _verificationLink = AppHelper._appUrl + "Home/ResetPassword?token=" + _user.UserToken;
                    await AppHelper.SendVerificationEmailAsync(Model.Email, "reset password", _verificationLink, _configuration);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at ForgetPassword (POST) in {_Title} Controller due to {ex}");
            }
            return View(Model);
        }
        //Reset Password (GET)
        public IActionResult ResetPassword(string token)
        {
            return View(new ResetPasswordVM() { UserToken = token });
        }
        //Reset Password (POST)
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM Model)
        {
            if (ModelState.IsValid)
            {
                var _user = _userService.GetUserByToken(Model.UserToken);
                var now = AppHelper.NowPakistan;
                if (_user != null && _user.TokenExpiryTime.HasValue && _user.TokenExpiryTime.Value >= now)
                {
                    _user.Password = Model.Password.Trim().HashWithSalt();
                    _user.UserToken = null; // Invalidate token to prevent reuse
                    _user.TokenExpiryTime = null;
                    _user.UpdatedDateTime = now;
                    _context.Entry(_user).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "1,Password Successfully Updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Message"] = "0,Invalid or expired password reset link. Please request a new one.";
                }
            }
            return View(Model);
        }
    }
}
