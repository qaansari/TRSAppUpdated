using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRoleService _roleService;
        private readonly string _Title = "User";
        private readonly int _UserID;
        public UserController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            ILogger<UserController> logger, IWebHostEnvironment webHostEnvironment,
            IRoleService roleService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _roleService = roleService;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
        }
        //Index (Get All)
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> Index(int _pageNo = 1, int _pageSize = 10, string _searchedTerm = "")
        {
            try
            {
                var (_result, totalRecords) = await GetList(_pageNo, _pageSize, _searchedTerm);
                ViewBag.TotalRecords = totalRecords;
                ViewBag.PageNo = _pageNo;
                ViewBag.PageSize = _pageSize;
                ViewBag.SearchedTerm = _searchedTerm;
                return View(_result ?? new List<UserVM>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index in {_Title} Controller due to {ex}");
            }
            return View(new List<UserVM>());
        }
        // GetPartial
        [Authorize(Roles = "Super Admin")]
        [HttpGet]
        public async Task<IActionResult> GetPartial(int _pageNo = 1, int _pageSize = 10, string _searchedTerm = "")
        {
            var _partialViewName = _Title.Contains(" ") ? _Title.Replace(" ", "") : _Title;
            try
            {
                var (_result, totalRecords) = await GetList(_pageNo, _pageSize, _searchedTerm);
                ViewBag.TotalRecords = totalRecords;
                ViewBag.PageNo = _pageNo;
                ViewBag.PageSize = _pageSize;
                ViewBag.SearchedTerm = _searchedTerm;
                return PartialView($"_{_partialViewName}TablePartial", _result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetPartial in {_Title} Controller due to {ex}");
            }
            return PartialView($"_{_partialViewName}TablePartial", new List<UserVM>());
        }
        //AddOrEdit (GET)
        [Authorize(Roles = "Super Admin")]
        public IActionResult AddOrEdit(int ID = 0)
        {
            var _item = new UserVM();
            try
            {
                ViewData["RoleList"] = _roleService.GetAll();
                if (ID != 0)
                {
                    _item = _context.Users.Where(x => !x.IsDeleted && x.ID == ID)
                        .Select(x => new UserVM()
                        {
                            ID = x.ID,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            ContactNumber = x.ContactNumber,
                            Password = x.Password,
                            ImagePath = x.ImagePath,
                            Email = x.Email,
                            IsActive = x.IsActive,
                            IsVerified = x.IsVerified,
                            RoleID = x.RoleID,
                            CreatedByUserID = x.CreatedByUserID,
                            CreatedDateTime = x.CreatedDateTime,
                        })
                        .AsNoTracking().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at AddOrEdit (GET) in {_Title} Controller due to {ex}");
            }
            return View(_item);
        }
        //AddOrEdit (POST)
        [Authorize(Roles = "Super Admin")]
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(UserVM Model)
        {
            try
            {
                ViewData["RoleList"] = _roleService.GetAll();
                if (ModelState.IsValid)
                {
                    var _item = new User();
                    if (Model.ID != 0)
                    {
                        var isExists = _context.Users.Where(x => !x.IsDeleted
                            && x.ID != Model.ID
                            && x.Email.Trim().ToLower() == Model.Email.Trim().ToLower())
                        .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item = _context.Users.Where(x => !x.IsDeleted && x.ID == Model.ID)
                            .AsNoTracking().FirstOrDefault();
                        if (_item != null)
                        {
                            _item.FirstName = Model.FirstName.Trim();
                            _item.LastName = Model.LastName.Trim();
                            _item.ContactNumber = Model.ContactNumber.Trim();
                            _item.RoleID = Model.RoleID;
                            _item.Email = Model.Email.Trim();
                            _item.IsActive = Model.IsActive;
                            _item.IsVerified = Model.IsVerified;
                            _item.UpdatedByUserID = _UserID;
                            _item.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                            _context.ChangeTracker.Clear();
                            _context.Entry(_item).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                            TempData["Message"] = $"1,{_Title} Successfully Updated";
                        }
                    }
                    else
                    {
                        var isExists = _context.Users.Where(x => !x.IsDeleted
                            && x.Email.Trim().ToLower() == Model.Email.Trim().ToLower())
                        .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item.FirstName = Model.FirstName.Trim();
                        _item.LastName = Model.LastName.Trim();
                        _item.ContactNumber = Model.ContactNumber.Trim();
                        _item.RoleID = Model.RoleID;
                        _item.Email = Model.Email.Trim();
                        _item.Password = Model.Password.Trim().HashWithSalt();
                        _item.IsActive = Model.IsActive;
                        _item.IsVerified = Model.IsVerified;
                        _item.CreatedByUserID = _UserID;
                        _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.Users.Add(_item);
                        await _context.SaveChangesAsync();
                        TempData["Message"] = $"1,{_Title} Successfully Added";
                    }
                    if (_item != null && Model.ImageFile != null)
                    {
                        var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "users");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        var _fileName = $"user" + "_" + _item.CreatedDateTime.Value.ToString("yyyyddMMhhmmss") + Path.GetExtension(Model.ImageFile.FileName);
                        _item.ImagePath = _fileName.Trim();
                        _item.UpdatedByUserID = _UserID;
                        _item.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                        var _filePath = Path.Combine(folderPath, _fileName);
                        var compressedBytes = await AppHelper.CompressImageAsync(Model.ImageFile);
                        await System.IO.File.WriteAllBytesAsync(_filePath, compressedBytes);
                        _context.ChangeTracker.Clear();
                        _context.Entry(_item).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error occured at AddOrEdit (POST) in {_Title} Controller due to {ex}");
            }
            return View(Model);
        }
        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> Delete(int ID)
        {
            try
            {
                var _item = _context.Users.Where(x => !x.IsDeleted && x.ID == ID)
                                  .AsNoTracking().FirstOrDefault();
                if (_item != null)
                {
                    _item.IsDeleted = true;
                    _item.DeletedByUserID = _UserID;
                    _item.DeletedDateTime = DateTime.UtcNow.AddHours(5);
                    _context.ChangeTracker.Clear();
                    _context.Entry(_item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"1,{_Title} Successfully Deleted";
                }
                else
                {
                    TempData["Message"] = $"0,No {_Title} Found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Delete (POST) in {_Title} Controller due to {ex}");
            }
            return RedirectToAction("Index");
        }
        //Get List
        [NonAction]
        [Authorize(Roles = "Super Admin")]
        public async Task<(List<UserVM> _result, int totalRecords)> GetList(int _pageNo, int _pageSize, string _searchedTerm)
        {

            var _result = new List<UserVM>();
            int totalRecords = 0;
            try
            {
                // For AJAX calls
                var _query = _context.Users.Include(x => x.Role).Where(x => !x.IsDeleted)
                           .Select(x => new UserVM()
                           {
                               ID = x.ID,
                               FirstName = x.FirstName,
                               LastName = x.LastName,
                               ContactNumber = x.ContactNumber,
                               Email = x.Email,
                               ImagePath = x.ImagePath,
                               RoleTitle = x.Role.Title,
                           })
                           .AsNoTracking()
                            .AsEnumerable();
                if (!string.IsNullOrEmpty(_searchedTerm))
                {
                    _query = _query
                        .Where(x => x.FullName.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                        || x.ContactNumber.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                        || x.Email.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                        || x.RoleTitle.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower()));
                }
                int _skip = (_pageNo - 1) * _pageSize;
                totalRecords = _query.Count();
                _query = _query.Skip(_skip).Take(_pageSize);
                _result = _query.OrderByDescending(x => x.ID).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetList in {_Title} Controller due to {ex}");
            }
            return (_result, totalRecords);
        }
        
    }
}
