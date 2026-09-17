using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    [Authorize(Roles = "Super Admin")]
    public class CountryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CountryController> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Country";
        private readonly int _UserID;

        public CountryController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            ILogger<CountryController> logger, ICacheService cacheService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _cacheService = cacheService;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
        }

        //Index (Get All)
        public async Task<IActionResult> Index(int _pageNo = 1, int _pageSize = 10, string _searchedTerm = "")
        {
            try
            {
                var (_result, totalRecords) = await GetList(_pageNo, _pageSize, _searchedTerm);
                ViewBag.TotalRecords = totalRecords;
                ViewBag.PageNo = _pageNo;
                ViewBag.PageSize = _pageSize;
                ViewBag.SearchedTerm = _searchedTerm;
                return View(_result ?? new List<CountryVM>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index in {_Title} Controller due to {ex}");
            }
            return View(new List<CountryVM>());
        }

        // GetPartial
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
            return PartialView($"_{_partialViewName}TablePartial", new List<CountryVM>());
        }

        //AddOrEdit (GET)
        public IActionResult AddOrEdit(int ID = 0)
        {
            var _item = new CountryVM();
            try
            {
                if (ID != 0)
                {
                    _item = _context.Countries.Where(x => !x.IsDeleted && x.ID == ID)
                        .Select(x => new CountryVM()
                        {
                            ID = x.ID,
                            Title = x.Title,
                            Flag = x.Flag,
                            DialingCode = x.DialingCode,
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
        public async Task<IActionResult> AddOrEdit(CountryVM Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _item = new Country();
                    if (Model.ID != 0)
                    {
                        var isExists = _context.Countries.Where(x => !x.IsDeleted
                            && x.ID != Model.ID
                            && x.Title.Trim().ToLower() == Model.Title.Trim().ToLower())
                        .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item = _context.Countries.Where(x => !x.IsDeleted && x.ID == Model.ID)
                            .AsNoTracking().FirstOrDefault();
                        if (_item != null)
                        {
                            _item.Title = Model.Title.Trim();
                            _item.DialingCode = Model.DialingCode.Trim();
                            _item.Flag = Model.Flag.Trim();
                            _item.UpdatedByUserID = _UserID;
                            _item.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                            _context.ChangeTracker.Clear();
                            _context.Entry(_item).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                            _cacheService.Remove(CacheKeys.Countries);
                            TempData["Message"] = $"1,{_Title} Successfully Updated";
                        }
                    }
                    else
                    {
                        var isExists = _context.Countries.Where(x => !x.IsDeleted
                            && x.Title.Trim().ToLower() == Model.Title.Trim().ToLower())
                        .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item.Title = Model.Title.Trim();
                        _item.DialingCode = Model.DialingCode.Trim();
                        _item.Flag = Model.Flag.Trim();
                        _item.CreatedByUserID = _UserID;
                        _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.Countries.Add(_item);
                        await _context.SaveChangesAsync();
                        _cacheService.Remove(CacheKeys.Countries);
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
                var _item = _context.Countries.Where(x => !x.IsDeleted && x.ID == ID)
                                  .AsNoTracking().FirstOrDefault();
                if (_item != null)
                {
                    _item.IsDeleted = true;
                    _item.DeletedByUserID = _UserID;
                    _item.DeletedDateTime = DateTime.UtcNow.AddHours(5);
                    _context.ChangeTracker.Clear();
                    _context.Entry(_item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    _cacheService.Remove(CacheKeys.Countries);
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
        public async Task<(List<CountryVM> _result, int totalRecords)> GetList(int _pageNo, int _pageSize, string _searchedTerm)
        {
            var _result = new List<CountryVM>();
            int totalRecords = 0;
            try
            {
                var _query = _context.Countries.Where(x => !x.IsDeleted)
                           .Select(x => new CountryVM()
                           {
                               ID = x.ID,
                               Title = x.Title,
                               Flag = x.Flag,
                               DialingCode = x.DialingCode
                           })
                           .AsNoTracking()
                           .AsQueryable();
                if (!string.IsNullOrEmpty(_searchedTerm))
                {
                    _query = _query
                        .Where(x => x.Title.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower()));
                }
                totalRecords = await _query.CountAsync();
                int _skip = (_pageNo - 1) * _pageSize;

                // Fixed: OrderBy before Skip/Take
                _result = await _query.OrderByDescending(x => x.ID).Skip(_skip).Take(_pageSize).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetList in {_Title} Controller due to {ex}");
            }
            return (_result, totalRecords);
        }
    }
}
