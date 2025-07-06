using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    [Authorize(Roles = "Super Admin")]
    public class EventController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<EventController> _logger;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;
        private readonly IVenueService _venueService;
        private readonly IAuditoriumService _auditoriumService;
        private readonly IEventStatusService _eventStatusService;
        private readonly string _Title = "Event";
        private readonly int _UserID;
        public EventController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment, ILogger<EventController> logger,
            ICountryService countryService, ICityService cityService,
            IVenueService venueService, IAuditoriumService auditoriumService,
            IEventStatusService eventStatusService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _countryService = countryService;
            _cityService = cityService;
            _venueService = venueService;
            _auditoriumService = auditoriumService;
            _eventStatusService = eventStatusService;
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
                return View(_result ?? new List<EventVM>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index in {_Title} Controller due to {ex}");
            }
            return View(new List<EventVM>());
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
            return PartialView($"_{_partialViewName}TablePartial", new List<EventVM>());
        }
        //AddOrEdit (GET)
        public IActionResult AddOrEdit(int ID = 0)
        {
            var _item = new EventVM();
            try
            {
                ViewData["CountryList"] = _countryService.GetAll();
                ViewData["CityList"] = _cityService.GetAll();
                ViewData["VenueList"] = _venueService.GetAll();
                ViewData["AuditoriumList"] = _auditoriumService.GetAll();
                ViewData["EventStatusList"] = _eventStatusService.GetAll();
                if (ID != 0)
                {
                    _item = _context.Events.Include(x => x.Auditorium).Include(x => x.City).Where(x => !x.IsDeleted && x.ID == ID)
                        .Select(x => new EventVM()
                        {
                            ID = x.ID,
                            Title = x.Title,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate,
                            Description = x.Description,
                            IsGeneral = x.IsGeneral,
                            ImagePath = x.ImagePath,
                            EventStatusID = x.EventStatusID,
                            CountryID = x.City.CountryID,
                            CityID = x.CityID,
                            VenueID = x.Auditorium.VenueID,
                            AuditoriumID = x.AuditoriumID,
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
        public async Task<IActionResult> AddOrEdit(EventVM Model)
        {
            try
            {
                ViewData["CountryList"] = _countryService.GetAll();
                ViewData["CityList"] = _cityService.GetAll();
                ViewData["VenueList"] = _venueService.GetAll();
                ViewData["AuditoriumList"] = _auditoriumService.GetAll();
                ViewData["EventStatusList"] = _eventStatusService.GetAll();
                if (ModelState.IsValid)
                {
                    var _item = new Event();
                    if (Model.ID != 0)
                    {
                        var isExists = await _context.Events
                        .Where(x => !x.IsDeleted
                        && x.ID != Model.ID
                        && x.CityID == Model.CityID
                        && x.AuditoriumID == Model.AuditoriumID
                        && x.Title.Trim().ToLower() == Model.Title.Trim().ToLower()
                        && (
                                (x.EndDate.HasValue && Model.EndDate.HasValue &&
                                 x.StartDate <= Model.EndDate.Value &&
                                 Model.StartDate.Value <= x.EndDate.Value)
                                 || (!x.EndDate.HasValue && Model.EndDate.HasValue &&
                                 x.StartDate <= Model.EndDate.Value &&
                                 Model.StartDate.Value <= x.StartDate)
                                 || (x.EndDate.HasValue && !Model.EndDate.HasValue &&
                                 Model.StartDate.Value <= x.EndDate.Value &&
                                 x.StartDate <= Model.StartDate.Value)
                                 || (!x.EndDate.HasValue && !Model.EndDate.HasValue &&
                                    x.StartDate == Model.StartDate.Value)
                            )
                            )
                        .AnyAsync();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item = _context.Events.Where(x => !x.IsDeleted && x.ID == Model.ID)
                            .AsNoTracking().FirstOrDefault();
                        if (_item != null)
                        {
                            _item.Title = Model.Title.Trim();
                            _item.CountryID = Model.CountryID;
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
                        var isExists = await _context.Events
                         .Where(x => !x.IsDeleted
                         && x.CityID == Model.CityID
                         && x.AuditoriumID == Model.AuditoriumID
                         && x.Title.Trim().ToLower() == Model.Title.Trim().ToLower()
                         && (
                                 (x.EndDate.HasValue && Model.EndDate.HasValue &&
                                  x.StartDate <= Model.EndDate.Value &&
                                  Model.StartDate.Value <= x.EndDate.Value)
                                  || (!x.EndDate.HasValue && Model.EndDate.HasValue &&
                                  x.StartDate <= Model.EndDate.Value &&
                                  Model.StartDate.Value <= x.StartDate)
                                  || (x.EndDate.HasValue && !Model.EndDate.HasValue &&
                                  Model.StartDate.Value <= x.EndDate.Value &&
                                  x.StartDate <= Model.StartDate.Value)
                                  || (!x.EndDate.HasValue && !Model.EndDate.HasValue &&
                                     x.StartDate == Model.StartDate.Value)
                             )
                             )
                         .AnyAsync();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item.Title = Model.Title.Trim();
                        _item.StartDate = Model.StartDate.Value;
                        _item.EndDate = Model.EndDate;
                        _item.Description = !string.IsNullOrEmpty(Model.Description)? Model.Description.Trim(): Model.Description;
                        _item.IsGeneral = Model.IsGeneral;
                        _item.CityID = Model.CityID;
                        _item.AuditoriumID = Model.AuditoriumID;
                        _item.CreatedByUserID = _UserID;
                        _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.Events.Add(_item);
                        await _context.SaveChangesAsync();
                        TempData["Message"] = $"1,{_Title} Successfully Added";
                    }
                    if (_item != null && Model.ImageFile != null)
                    {
                        var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "auditoriums");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        var _fileName = $"auditorium" + "_" + _item.CreatedDateTime.ToString("yyyyddMMhhmmss") + Path.GetExtension(Model.ImageFile.FileName);
                        _item.ImagePath = _fileName.Trim();
                        var _filePath = Path.Combine(folderPath, _fileName);
                        using (var _stream = new FileStream(_filePath, FileMode.Create))
                        {
                            await Model.ImageFile.CopyToAsync(_stream);
                        }
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
        public async Task<IActionResult> Delete(int ID)
        {
            try
            {
                var _item = _context.Events.Where(x => !x.IsDeleted && x.ID == ID)
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
        public async Task<(List<EventVM> _result, int totalRecords)> GetList(int _pageNo, int _pageSize, string _searchedTerm)
        {

            var _result = new List<EventVM>();
            int totalRecords = 0;
            try
            {
                // For AJAX calls
                var _query = _context.Events.Include(x => x.Country).Where(x => !x.IsDeleted)
                           .Select(x => new EventVM()
                           {
                               ID = x.ID,
                               Title = x.Title,
                               CountryTitle = x.Country.Title
                           })
                           .AsNoTracking()
                           .AsQueryable();
                if (!string.IsNullOrEmpty(_searchedTerm))
                {
                    _query = _query
                        .Where(x => x.Title.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                        || x.CountryTitle.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower()));
                }
                int _skip = (_pageNo - 1) * _pageSize;
                totalRecords = await _query.CountAsync();
                _query = _query.Skip(_skip).Take(_pageSize);
                _result = await _query.OrderByDescending(x => x.ID).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetList in {_Title} Controller due to {ex}");
            }
            return (_result, totalRecords);
        }
    }
