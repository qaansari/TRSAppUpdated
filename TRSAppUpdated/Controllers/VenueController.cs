using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.FileManager.Base;
using Syncfusion.EJ2.Layouts;
using System.Security.Claims;
using System.Threading.Tasks;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    [Authorize(Roles = "Super Admin")]
    public class VenueController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<VenueController> _logger;
        private readonly string _Title = "Venue";
        private readonly int _UserID;
        public VenueController(AppDbContext context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, ILogger<VenueController> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
        }
        //Index (Get All)
        public async Task<IActionResult> Index(int _pageNo = 1, int _pageSize = 1, string _searchedTerm = "")
        {
            try
            {
                var (_result, totalRecords) = await GetList(_pageNo, _pageSize, _searchedTerm);
                ViewBag.TotalRecords = totalRecords;
                ViewBag.PageNo = _pageNo;
                ViewBag.PageSize = _pageSize;
                ViewBag.SearchedTerm = _searchedTerm;
                return View(_result ?? new List<VenueVM>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index in {_Title} Controller due to {ex}");
            }
            return View(new List<VenueVM>());
        }
        // GetPartial
        [HttpGet]
        public async Task<IActionResult> GetPartial(int _pageNo = 1, int _pageSize = 1, string _searchedTerm = "")
        {
            try
            {
                var (_result, totalRecords) = await GetList(_pageNo, _pageSize, _searchedTerm);
                ViewBag.TotalRecords = totalRecords;
                ViewBag.PageNo = _pageNo;
                ViewBag.PageSize = _pageSize;
                ViewBag.SearchedTerm = _searchedTerm;
                return PartialView("_VenuesTablePartial", _result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetVenuesPartial in {_Title} Controller due to {ex}");
            }
            return PartialView("_VenuesTablePartial", new List<VenueVM>());
        }
        //AddOrEdit (GET)
        public IActionResult AddOrEdit(int ID = 0)
        {
            var _item = new VenueVM();
            try
            {
                if (ID != 0)
                {
                    _item = _context.Venues.Where(x => !x.IsDeleted && x.ID == ID)
                        .Select(x => new VenueVM()
                        {
                            ID = x.ID,
                            Title = x.Title,
                            Address = x.Address,
                            ImagePath = x.ImagePath,
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
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(VenueVM Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _item = new Venue();
                    if (Model.ImageFile != null)
                    {
                        var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "venues");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        var _fileName = $"venue" + "_" + Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(Model.ImageFile.FileName);
                        Model.ImagePath = _fileName.Trim();
                        var _filePath = Path.Combine(folderPath, _fileName);
                        using (var _stream = new FileStream(_filePath, FileMode.Create))
                        {
                            await Model.ImageFile.CopyToAsync(_stream);
                        }
                    }
                    if (Model.ID != 0)
                    {
                        _item = _context.Venues.Where(x => !x.IsDeleted && x.ID == Model.ID)
                            .AsNoTracking().FirstOrDefault();
                        if (_item != null)
                        {
                            _item.Title = Model.Title.Trim();
                            _item.Address = Model.Address.Trim();
                            _item.ImagePath = Model.ImagePath;
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
                        _item.Title = Model.Title.Trim();
                        _item.Address = Model.Address.Trim();
                        _item.ImagePath = Model.ImagePath;
                        _item.CreatedByUserID = _UserID;
                        _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.Venues.Add(_item);
                        await _context.SaveChangesAsync();
                        TempData["Message"] = $"1,{_Title} Successfully Added";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error occured at AddOrEdit (POST) in {_Title} Controller due to {ex}");
            }
            return View(Model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int ID)
        {
            try
            {
                var _item = _context.Venues.Where(x => !x.IsDeleted && x.ID == ID)
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
        [NonAction]
        public async Task<(List<VenueVM> _result, int totalRecords)> GetList(int _pageNo, int _pageSize, string _searchedTerm)

        {
            var _result = new List<VenueVM>();
            int totalRecords = 0;
            // For AJAX calls
            var _query = _context.Venues.Where(x => !x.IsDeleted)
                       .Select(x => new VenueVM()
                       {
                           ID = x.ID,
                           Title = x.Title,
                           Address = x.Address,
                           ImagePath = x.ImagePath,
                       })
                       .AsNoTracking()
                       .AsQueryable();
            if (!string.IsNullOrEmpty(_searchedTerm))
            {
                _query = _query
                    .Where(x => x.Title.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                || x.Address.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower()));
            }
            int _skip = (_pageNo - 1) * _pageSize;
            totalRecords = await _query.CountAsync();
            _query = _query.Skip(_skip).Take(_pageSize);
            _result = await _query.ToListAsync();
            return (_result, totalRecords);
        }
    }
}
