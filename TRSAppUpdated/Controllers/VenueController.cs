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
    public class VenueController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<VenueController> _logger;
        private readonly ICityService _cityService;
        private readonly ICountryService _countryService;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Venue";
        private readonly int _UserID;

        public VenueController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment, ILogger<VenueController> logger, ICityService cityService,
            ICountryService countryService, ICacheService cacheService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _countryService = countryService;
            _cityService = cityService;
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
            return PartialView($"_{_partialViewName}TablePartial", new List<VenueVM>());
        }

        //AddOrEdit (GET)
        public IActionResult AddOrEdit(int ID = 0)
        {
            var _item = new VenueVM();
            try
            {
                ViewData["CountryList"] = _countryService.GetAll();
                ViewData["CityList"] = _cityService.GetAll();
                if (ID != 0)
                {
                    _item = _context.Venues.Where(x => !x.IsDeleted && x.ID == ID)
                        .Select(x => new VenueVM()
                        {
                            ID = x.ID,
                            Title = x.Title,
                            CountryID = x.City.CountryID,
                            CityID = x.CityID,
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
                ViewData["CountryList"] = _countryService.GetAll();
                ViewData["CityList"] = _cityService.GetAll();
                if (Model.ImageFile != null && !AppHelper.IsValidImageFile(Model.ImageFile, out var imgError))
                {
                    TempData["Message"] = $"0,{imgError}";
                    return View(Model);
                }
                if (ModelState.IsValid)
                {
                    var _item = new Venue();
                    if (Model.ID != 0)
                    {
                        var isExists = _context.Venues.Where(x => !x.IsDeleted
                            && x.ID != Model.ID
                            && x.CityID == Model.CityID
                            && x.Title.Trim().ToLower() == Model.Title.Trim().ToLower())
                        .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item = _context.Venues.Where(x => !x.IsDeleted && x.ID == Model.ID)
                            .AsNoTracking().FirstOrDefault();
                        if (_item != null)
                        {
                            _item.Title = Model.Title.Trim();
                            _item.CityID = Model.CityID;
                            _item.Address = Model.Address.Trim();
                            _item.UpdatedByUserID = _UserID;
                            _item.UpdatedDateTime = AppHelper.NowPakistan;
                            _context.ChangeTracker.Clear();
                            _context.Entry(_item).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                            _cacheService.Remove(CacheKeys.Venues);
                            _cacheService.RemoveByPrefix(CacheKeys.EventDetailsPrefix);
                            _cacheService.RemoveByPrefix(CacheKeys.ViewTicketsPrefix);
                            TempData["Message"] = $"1,{_Title} Successfully Updated";
                        }
                    }
                    else
                    {
                        var isExists = _context.Venues.Where(x => !x.IsDeleted
                            && x.CityID == Model.CityID
                            && x.Title.Trim().ToLower() == Model.Title.Trim().ToLower())
                        .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item.Title = Model.Title.Trim();
                        _item.CityID = Model.CityID;
                        _item.Address = Model.Address.Trim();
                        _item.CreatedByUserID = _UserID;
                        _item.CreatedDateTime = AppHelper.NowPakistan;
                        _context.Venues.Add(_item);
                        await _context.SaveChangesAsync();
                        _cacheService.Remove(CacheKeys.Venues);
                        _cacheService.RemoveByPrefix(CacheKeys.EventDetailsPrefix);
                        _cacheService.RemoveByPrefix(CacheKeys.ViewTicketsPrefix);
                        TempData["Message"] = $"1,{_Title} Successfully Added";
                    }
                    if (_item != null && Model.ImageFile != null)
                    {
                        var savedFileName = await AppHelper.SaveUploadedImageAsync(Model.ImageFile, _webHostEnvironment.WebRootPath, "venues", "venue");
                        if (!string.IsNullOrEmpty(savedFileName))
                        {
                            _item.ImagePath = savedFileName;
                            _item.UpdatedByUserID = _UserID;
                            _item.UpdatedDateTime = AppHelper.NowPakistan;
                            _context.ChangeTracker.Clear();
                            _context.Entry(_item).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                        }
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
                    _item.DeletedDateTime = AppHelper.NowPakistan;
                    _context.ChangeTracker.Clear();
                    _context.Entry(_item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    _cacheService.Remove(CacheKeys.Venues);
                    _cacheService.RemoveByPrefix(CacheKeys.EventDetailsPrefix);
                    _cacheService.RemoveByPrefix(CacheKeys.ViewTicketsPrefix);
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
        public async Task<(List<VenueVM> _result, int totalRecords)> GetList(int _pageNo, int _pageSize, string _searchedTerm)
        {
            var _result = new List<VenueVM>();
            int totalRecords = 0;
            try
            {
                var _query = _context.Venues
                    .Where(x => !x.IsDeleted)
                           .Select(x => new VenueVM()
                           {
                               ID = x.ID,
                               Title = x.Title,
                               CountryTitle = x.City.Country.Title,
                               CityTitle = x.City.Title,
                               Address = x.Address,
                               ImagePath = x.ImagePath,
                           })
                           .AsNoTracking()
                           .AsQueryable();
                if (!string.IsNullOrEmpty(_searchedTerm))
                {
                    string term = _searchedTerm.Trim().ToLower();
                    _query = _query
                        .Where(x => x.Title.Trim().ToLower().Contains(term)
                    || x.Address.Trim().ToLower().Contains(term)
                    || x.CityTitle.Trim().ToLower().Contains(term)
                    || x.CountryTitle.Trim().ToLower().Contains(term));
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
