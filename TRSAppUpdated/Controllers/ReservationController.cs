using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using Syncfusion.EJ2.FileManager.Base;
using Syncfusion.EJ2.TreeGrid;
using System.Globalization;
using System.Security.Claims;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPdfService _pdfService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ReservationController> _logger;
        private readonly IReservationService _reservationService;
        private readonly IShowService _showService;
        private readonly ITicketService _ticketService;
        private readonly IPaymentModeService _paymentModeService;
        private readonly IReservationStatusService _reservationStatusService;
        private readonly IUserService _userService;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Reservation";
        private readonly int _UserID;
        public ReservationController(AppDbContext context, IHttpContextAccessor httpContextAccessor, IPdfService pdfService,
            IWebHostEnvironment webHostEnvironment, ILogger<ReservationController> logger,
            IReservationService reservationService, IShowService showService,
            ITicketService ticketService, IPaymentModeService paymentModeService,
            IReservationStatusService reservationStatusService, IUserService userService,
            ICacheService cacheService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _pdfService = pdfService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _reservationService = reservationService;
            _showService = showService;
            _ticketService = ticketService;
            _paymentModeService = paymentModeService;
            _reservationStatusService = reservationStatusService;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
            _userService = userService;
            _cacheService = cacheService;
        }
        [Authorize(Roles = "Super Admin,Admin")]
        public async Task<IActionResult> AddOrEdit(int ID = 0, int EventID = 0)
        {
            var item = new ReservationVM();
            try
            {
                if (ID != 0)
                {
                    item = await _reservationService.GetForEditAsync(ID, EventID) ?? new ReservationVM { ID = ID, EventID = EventID };
                    if (EventID == 0 && item.EventID != 0)
                    {
                        EventID = item.EventID;
                    }
                }
                else
                {
                    item.EventID = EventID;
                    item.TicketNumber = Guid.NewGuid().ToString().Substring(0, 7).ToUpper();
                }

                PopulateDropDowns(EventID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at AddOrEdit (GET) in {Title} Controller", _Title);
            }
            return View(item);
        }

        [Authorize(Roles = "Super Admin,Admin")]
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(ReservationVM Model)
        {
            try
            {
                NormalizeDateTimes(Model);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(e => !string.IsNullOrEmpty(e))
                        .ToList();

                    TempData["Message"] = "0," + (errors.Count > 0 ? string.Join("; ", errors) : "Please correct the form errors and try again.");
                    PopulateDropDowns(Model.EventID);
                    return View(Model);
                }

                bool isSuperAdmin = User.IsInRole("Super Admin");
                var result = await _reservationService.SaveReservationAsync(Model, _UserID, isSuperAdmin);

                TempData["Message"] = (result.Success ? "1," : "0,") + result.Message;

                if (result.Success)
                {
                    return RedirectToAction("Details", "Event", new { ID = result.EventId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at AddOrEdit (POST) in {Title} Controller", _Title);
                TempData["Message"] = "0,An unexpected error occurred while saving the reservation.";
            }

            PopulateDropDowns(Model.EventID);
            return View(Model);
        }

        private void PopulateDropDowns(int eventId)
        {
            ViewBag.EventID = eventId;
            ViewData["TicketList"] = _ticketService.GetTicketsByEventID(eventId);
            ViewData["ShowList"] = _showService.GetAllShowsByEventID(eventId);
            ViewData["PaymentModeList"] = _paymentModeService.GetAll();
            ViewData["ReservationStatusList"] = _reservationStatusService.GetAll();
            ViewData["UserList"] = _userService.GetAll();
        }

        private void NormalizeDateTimes(ReservationVM model)
        {
            var cultures = new[]
            {
                CultureInfo.GetCultureInfo("en-US"),
                CultureInfo.InvariantCulture,
                CultureInfo.CurrentCulture
            };

            // Recover ActionDateTime if model binding failed
            if (ModelState.TryGetValue(nameof(model.ActionDateTime), out var actionState) && actionState.Errors.Count > 0)
            {
                var rawValue = actionState.AttemptedValue;
                if (!string.IsNullOrWhiteSpace(rawValue))
                {
                    foreach (var culture in cultures)
                    {
                        if (DateTime.TryParse(rawValue, culture, DateTimeStyles.None, out var parsedDate))
                        {
                            model.ActionDateTime = parsedDate;
                            actionState.Errors.Clear();
                            actionState.RawValue = parsedDate;
                            break;
                        }
                    }
                }
            }

            // Recover BookingDateTime if model binding failed
            if (ModelState.TryGetValue(nameof(model.BookingDateTime), out var bookingState) && bookingState.Errors.Count > 0)
            {
                var rawValue = bookingState.AttemptedValue;
                if (!string.IsNullOrWhiteSpace(rawValue))
                {
                    foreach (var culture in cultures)
                    {
                        if (DateTime.TryParse(rawValue, culture, DateTimeStyles.None, out var parsedDate))
                        {
                            model.BookingDateTime = parsedDate;
                            bookingState.Errors.Clear();
                            bookingState.RawValue = parsedDate;
                            break;
                        }
                    }
                }
            }
        }
        //Details
        [Authorize(Roles = "Super Admin,Admin,Moderator,User")]
        public async Task<IActionResult> Details(int ID = 0, int EventID = 0)
        {
            var _item = new ReservationDetailVM();
            try
            {
                if (EventID != 0)
                {
                    ViewBag.EventID = EventID;
                    ViewBag.IsGeneralEvent = await _context.Events
                        .Where(x => !x.IsDeleted && x.ID == EventID)
                        .AsNoTracking()
                        .Select(x => x.IsGeneral)
                        .FirstOrDefaultAsync();
                }
                if (ID != 0)
                {
                    _item = await _reservationService.GetDetailsByIDAsync(ID);
                    if (!User.IsInRole("Super Admin") && !User.IsInRole("Admin") && _item.CreatedByUserID != _UserID)
                    {
                        return Forbid();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at Details in {Title} Controller", _Title);
            }
            return View(_item);
        }
        [Authorize(Roles = "Super Admin,Admin,Moderator,User")]
        [EnableRateLimiting("pdf-limit")]
        public async Task<IActionResult> GetPDF(int ID = 0, int EventID = 0)
        {
            var _item = new ReservationDetailVM();
            try
            {
                if (EventID != 0)
                    ViewBag.EventID = EventID;
                bool isGenEvent = await _context.Events
                       .Where(x => !x.IsDeleted && x.ID == EventID)
                       .AsNoTracking()
                       .Select(x => x.IsGeneral)
                       .FirstOrDefaultAsync();

                if (ID != 0)
                {
                    _item = await _reservationService.GetDetailsByIDAsync(ID);
                    if (!User.IsInRole("Super Admin") && !User.IsInRole("Admin") && _item.CreatedByUserID != _UserID)
                    {
                        return Forbid();
                    }
                }

                var _ticketTitle = _item.UpdatedDateTime.HasValue
                   ? $"E-TICKET_{_item.TicketNumber}_UPDATED"
                   : $"E-TICKET_{_item.TicketNumber}";

                byte[] pdf = _pdfService.GenerateTicketPdf(_item, isGenEvent, _webHostEnvironment.WebRootPath);
                return File(pdf, "application/pdf", $"{_ticketTitle}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred at GetPDF in {Title} Controller", _Title);
            }
            TempData["Message"] = "0,Error while generating PDF";
            return RedirectToAction("Details", "Event", new { ID = EventID });
        }
        //Close Reservation
        [Authorize(Roles = "Super Admin,Admin")]
        [HttpPost]
        public async Task<IActionResult> Close(int ID, int EventID)
        {
            var result = await _reservationService.CloseReservationAsync(ID, EventID, _UserID);
            TempData["Message"] = (result.Success ? "1," : "0,") + result.Message;
            return RedirectToAction("Details", "Event", new { ID = EventID });
        }

    }
}
