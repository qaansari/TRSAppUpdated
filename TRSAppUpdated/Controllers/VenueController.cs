using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Layouts;
using System.Security.Claims;
using System.Threading.Tasks;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
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
        public async Task<IActionResult> Index(int _pageNo = 1, int _pageSize = 50)
        {
            try
            {
                var _query = _context.Venues.Where(x => !x.IsDeleted)
                        .Select(x => new VenueVM()
                        {
                            Title = x.Title,
                            Address = x.Address,
                            ImagePath = x.ImagePath,
                        })
                        .OrderByDescending(x => x.ID)
                        .AsNoTracking()
                        .AsQueryable();
                int _skip = (_pageNo - 1) * _pageSize;
                ViewBag.TotalRecords = await _query.CountAsync();
                ViewBag.PageNo = _pageNo;
                ViewBag.PageSize = _pageSize;
                _query = _query.Take(_pageSize).Skip(_skip);
                var _result = await _query.ToListAsync();
                return View(_result ?? new List<VenueVM>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index in {_Title} Controller due to {ex}");
            }
            return View(new List<VenueVM>());
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
                        _context.ChangeTracker.Clear();
                        _context.Venues.Add(_item);
                        await _context.SaveChangesAsync();
                        TempData["Message"] = $"1,{_Title} Successfully Added";
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
    }
}
