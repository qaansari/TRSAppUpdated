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
    public class EventController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<EventController> _logger;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;
        private readonly IVenueService _venueService;
        private readonly ITicketService _ticketService;
        private readonly IAuditoriumService _auditoriumService;
        private readonly IEventStatusService _eventStatusService;
        private readonly ICategoryService _categoryService;
        private readonly IEventService _service;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Event";
        private readonly int _UserID;
        public EventController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment, ILogger<EventController> logger,
            ICountryService countryService, ICityService cityService,
            IVenueService venueService,
            ITicketService ticketService, IAuditoriumService auditoriumService,
            IEventStatusService eventStatusService, ICategoryService categoryService,
            IEventService service, ICacheService cacheService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _countryService = countryService;
            _cityService = cityService;
            _venueService = venueService;
            _ticketService = ticketService;
            _auditoriumService = auditoriumService;
            _eventStatusService = eventStatusService;
            _categoryService = categoryService;
            _service = service;
            _cacheService = cacheService;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
        }
        [Authorize(Roles = "Super Admin,Admin,Moderator,User")]
        //Index (Get All)
        public async Task<IActionResult> Index(int _pageNo = 1, int _pageSize = 10, string _searchedTerm = "")
        {
            try
            {
                var (_result, totalRecords) = await _service.GetAllPaginated(_pageNo, _pageSize, _searchedTerm);
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
                var (_result, totalRecords) = await _service.GetAllPaginated(_pageNo, _pageSize, _searchedTerm);
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
        [Authorize(Roles = "Super Admin,Admin")]
        public IActionResult AddOrEdit(int ID = 0)
        {
            var _item = new EventVM();
            try
            {
                ViewData["CountryList"] = _countryService.GetAll();
                ViewData["CityList"] = _cityService.GetAll();
                ViewData["VenueList"] = _venueService.GetAll();
                ViewData["AuditoriumList"] = _auditoriumService.GetAll();
                ViewData["CategoryList"] = _categoryService.GetAll();
                ViewData["EventStatusList"] = _eventStatusService.GetAll();
                if (ID != 0)
                {
                    _item = _context.Events
                        .Where(x => !x.IsDeleted && x.ID == ID)
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
                            CountryID = x.Auditorium.Venue.City.CountryID,
                            CityID = x.Auditorium.Venue.CityID,
                            VenueID = x.Auditorium.VenueID,
                            AuditoriumID = x.AuditoriumID,
                            EventCategoryIDs = x.EventCategories.Select(x => x.CategoryID).ToList(),
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
        [Authorize(Roles = "Super Admin,Admin")]
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(EventVM Model)
        {
            try
            {
                ViewData["CountryList"] = _countryService.GetAll();
                ViewData["CityList"] = _cityService.GetAll();
                ViewData["VenueList"] = _venueService.GetAll();
                ViewData["AuditoriumList"] = _auditoriumService.GetAll();
                ViewData["CategoryList"] = _categoryService.GetAll();
                ViewData["EventStatusList"] = _eventStatusService.GetAll();
                if (ModelState.IsValid)
                {
                    var _item = new Event();
                    if (Model.ID != 0)
                    {
                        var isExists = await _context.Events
                        .Where(x => !x.IsDeleted
                        && x.ID != Model.ID
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
                            _item.StartDate = Model.StartDate.Value;
                            _item.EndDate = Model.EndDate;
                            _item.Description = !string.IsNullOrEmpty(Model.Description) ? Model.Description.Trim() : Model.Description;
                            _item.IsGeneral = Model.IsGeneral;
                            if (Model.AuditoriumID != _item.AuditoriumID)
                            {
                                var _relatedReservationImages = _context.ReservationImages
                                 .Where(x => x.Reservation.Show.EventID == _item.ID)
                                 .AsNoTracking()
                                 .ToList();
                                _context.ReservationImages.RemoveRange(_relatedReservationImages);
                                var _relatedReservationTickets = _context.ReservationTickets
                                 .Where(x => x.Reservation.Show.EventID == _item.ID)
                                 .AsNoTracking()
                                 .ToList();
                                _context.ReservationTickets.RemoveRange(_relatedReservationTickets);
                                var _relatedReservations = _context.Reservations
                                   .Where(x => !x.IsDeleted && x.Show.EventID == _item.ID)
                                   .AsNoTracking()
                                   .ToList();
                                _context.Reservations.RemoveRange(_relatedReservations);
                                var _relatedTickets = _context.Tickets
                                  .Where(x => !x.IsDeleted && x.Show.EventID == _item.ID)
                                  .AsNoTracking()
                                  .ToList();
                                _context.Tickets.RemoveRange(_relatedTickets);
                                var _relatedShows = _context.Shows
                                    .Where(x => !x.IsDeleted && x.EventID == _item.ID)
                                    .AsNoTracking()
                                    .ToList();
                                _context.Shows.RemoveRange(_relatedShows);
                                await _context.SaveChangesAsync();
                            }
                            _item.AuditoriumID = Model.AuditoriumID;
                            _item.EventStatusID = Model.EventStatusID;
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
                        _item.Description = !string.IsNullOrEmpty(Model.Description) ? Model.Description.Trim() : Model.Description;
                        _item.IsGeneral = Model.IsGeneral;
                        _item.AuditoriumID = Model.AuditoriumID;
                        _item.EventStatusID = Model.EventStatusID;
                        _item.CreatedByUserID = _UserID;
                        _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.Events.Add(_item);
                        await _context.SaveChangesAsync();
                        TempData["Message"] = $"1,{_Title} Successfully Added";
                    }
                    if (_item != null)
                    {
                        if (Model.ImageFile != null)
                        {
                            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "events");
                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }
                            var _fileName = $"event" + "_" + _item.CreatedDateTime.ToString("yyyyddMMhhmmss") + Path.GetExtension(Model.ImageFile.FileName);
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
                        if (Model.EventCategoryIDs != null && Model.EventCategoryIDs.Count > 0)
                        {
                            var _existingEventCategories = _context.EventCategories
                            .Where(x => x.EventID == _item.ID)
                            .AsNoTracking()
                            .ToList();
                            if (_existingEventCategories.Count > 0)
                            {
                                _context.EventCategories.RemoveRange(_existingEventCategories);
                            }
                            foreach (var categoryID in Model.EventCategoryIDs)
                            {
                                _context.EventCategories.Add(new EventCategory()
                                {
                                    CategoryID = categoryID,
                                    EventID = _item.ID,
                                    CreatedByUserID = _UserID,
                                    CreatedDateTime = DateTime.UtcNow.AddHours(5),
                                });
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                    _cacheService.Remove(CacheKeys.EventsList);
                    _cacheService.Remove(CacheKeys.DashboardMetrics);
                    return RedirectToAction("Index");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error occured at AddOrEdit (POST) in {_Title} Controller due to {ex}");
            }
            return View(Model);
        }
        [Authorize(Roles = "Super Admin")]
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
                    _cacheService.Remove(CacheKeys.EventsList);
                    _cacheService.Remove(CacheKeys.DashboardMetrics);
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
        //End
        [Authorize(Roles = "Super Admin,Admin")]
        [HttpPost]
        public async Task<IActionResult> End(int ID)
        {
            try
            {
                var _status = _context.EventStatuses.Where(x => !x.IsDeleted && x.Title.ToLower() == "ended")
                                 .AsNoTracking().FirstOrDefault();
                var _item = _context.Events.Where(x => !x.IsDeleted && x.ID == ID)
                                  .AsNoTracking().FirstOrDefault();
                if (_item != null && _status != null)
                {
                    _item.EventStatusID = _status.ID;
                    _item.UpdatedByUserID = _UserID;
                    _item.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                    _context.ChangeTracker.Clear();
                    _context.Entry(_item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    _cacheService.Remove(CacheKeys.EventsList);
                    _cacheService.Remove(CacheKeys.DashboardMetrics);
                    TempData["Message"] = $"1,{_Title} Successfully Ended";
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
        //Details

        [Authorize(Roles = "Super Admin,Admin,Moderator,User")]
        public async Task<IActionResult> Details(int ID)
        {
            try
            {
                var _item = await _service.GetDetailsByID(ID);
                if (!_httpContextAccessor.HttpContext.User.IsInRole("Super Admin") && !_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
                {
                    _item.ReservationList = _item.ReservationList.Count > 0 ? _item.ReservationList.Where(x => x.CreatedByUserID == _UserID).ToList() : new List<ReservationVM>();
                }
                return View(_item ?? new EventVM());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Details in {_Title} Controller due to {ex}");
            }
            return View(new EventVM());
        }
    }
}
