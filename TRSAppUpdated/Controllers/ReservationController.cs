using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConverter _converter;
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
        public ReservationController(AppDbContext context, IHttpContextAccessor httpContextAccessor, IConverter converter,
            IWebHostEnvironment webHostEnvironment, ILogger<ReservationController> logger,
            IReservationService reservationService, IShowService showService,
            ITicketService ticketService, IPaymentModeService paymentModeService,
            IReservationStatusService reservationStatusService, IUserService userService,
            ICacheService cacheService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _converter = converter;
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
        public IActionResult Details(int ID = 0, int EventID = 0)
        {

            var _item = new ReservationDetailVM();
            try
            {
                if (EventID != 0)
                {
                    ViewBag.EventID = EventID;
                    ViewBag.IsGeneralEvent = _context.Events
                        .Where(x => !x.IsDeleted && x.ID == EventID)
                        .AsNoTracking()
                        .Select(x => x.IsGeneral)
                        .FirstOrDefault();
                }
                if (ID != 0)
                {
                    _item = _reservationService.GetDetailsByID(ID);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at AddOrEdit (GET) in {_Title} Controller due to {ex}");
            }
            return View(_item);
        }
        [Authorize(Roles = "Super Admin,Admin,Moderator,User")]
        public IActionResult GetPDF(int ID = 0, int EventID = 0)
        {
            var _item = new ReservationDetailVM();
            try
            {
                if (EventID != 0)
                    ViewBag.EventID = EventID;
                bool isGenEvent = _context.Events
                       .Where(x => !x.IsDeleted && x.ID == EventID)
                       .AsNoTracking()
                       .Select(x => x.IsGeneral)
                       .FirstOrDefault();
                

                if (ID != 0)
                    _item = _reservationService.GetDetailsByID(ID);

                var seatLabelsList = !string.IsNullOrEmpty(_item.SeatLabels)
                    ? _item.SeatLabels.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList()
                    : new List<string>();
                var seats = seatLabelsList.Count > 0
                    ? string.Join("", seatLabelsList.Select(s => $"<span class='seats-badge'>{s}</span>"))
                    : "<span class='seats-badge'>None</span>";
                if (isGenEvent)
                {
                    seats = $"<span class='seats-badge'>First Come First Sit</span>";
                }
                string _docWidth = "210mm";
                string _docHeight = "80mm";
                string logoImageSrc = GetImageSource("web-logo-light.png", "assets/images");
                string eventImageSrc = GetImageSource(_item.EventImagePath, "assets/images/events");

                bool hasEventImage = !string.IsNullOrEmpty(eventImageSrc);
                string infoColumnWidth = hasEventImage ? "58%" : "100%";
                string imageHtml = hasEventImage
                    ? $@"<div class='image_info'>
                            <img src='{eventImageSrc}' class='event_img'/>
                         </div>"
                    : "";
                string logoHtml = !string.IsNullOrEmpty(logoImageSrc)
                    ? $@"<div class='logo_img_section'>
                           <img src='{logoImageSrc}' class='logo_img' />
                        </div>"
                    : "";

                string html = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<style>
    @page {{
        size: {_docWidth} {_docHeight};
        margin: 0;
    }}
    * {{
        box-sizing: border-box;
        page-break-inside: avoid !important;
        page-break-after: avoid !important;
        page-break-before: avoid !important;
    }}
    ::-webkit-scrollbar {{
        display: none !important;
        width: 0px !important;
        height: 0px !important;
        background: transparent !important;
    }}
    html, body {{
        margin: 0;
        padding: 0;
        font-family: 'Poppins', Arial, sans-serif;
        background: #ffffff;
        width: 100%;
        height: 100%;
        overflow: hidden;
    }}
    .ticket-container {{
        width: 100%;
        margin: 0;
        padding: 0;
        background: #ffffff;
        overflow: hidden;
    }}
    .ticket-header {{
        background: linear-gradient(135deg, rgb(26,164,139), rgb(69,165,121));
        color: #ffffff;
        padding: 6px 15px;
        text-align: center;
    }}
    .ticket-header h2 {{
        margin: 0;
        font-size: 16px;
        font-weight: 600;
        letter-spacing: 1px;
    }}
    .ticket-body {{
        padding: 6px 16px 2px 16px;
    }}
    .logo_img_section {{
        text-align: center;
        margin-bottom: 2px;
    }}
    .logo_img {{
        max-height: 60px;
        max-width: 200px;
        width: auto;
        height: auto;
    }}
    .ticket-number {{
        background: rgb(248,249,250);
        padding: 4px 10px;
        border-radius: 5px;
        text-align: center;
        margin: 3px 0 6px 0;
        border: 1px dashed rgb(200,200,200);
    }}
    .ticket-number h3 {{
        margin: 0;
        font-size: 16px;
        color: rgb(26,164,139);
        letter-spacing: 2px;
        text-transform: uppercase;
        font-weight: 700;
    }}
    .ticket-content {{
        width: 100%;
    }}
    .ticket-info {{
        display: inline-block;
        width: {infoColumnWidth};
        vertical-align: top;
    }}
    .image_info {{
        display: inline-block;
        width: 39%;
        vertical-align: top;
        text-align: right;
    }}
    .event_img {{
        border-radius: 6px;
        width: 100%;
        max-height: 115px;
        height: auto;
        object-fit: cover;
        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        border: 1px solid #eaeaea;
    }}
    .info-table {{
        width: 100%;
        border-collapse: collapse;
    }}
    .info-table td {{
        padding: 2.5px 0;
        vertical-align: middle;
        font-size: 10px;
        border-bottom: 1px solid rgb(240,240,240);
    }}
    .info-table tr:last-child td {{
        border-bottom: none;
    }}
    .info-label {{
        font-weight: 600;
        width: 110px;
        color: rgb(40,40,40);
    }}
    .info-value {{
        color: rgb(15,15,15);
    }}
    .seats-badge {{
        display: inline-block;
        background: rgb(88,255,217);
        color: rgb(16,12,12);
        padding: 2px 5px;
        border-radius: 3px;
        font-weight: 600;
        font-size: 9.5px;
        margin-right: 2px;
        margin-bottom: 2px;
    }}
    .seat_category {{
        font-weight: 600;
        text-transform: uppercase;
    }}
    .footer {{
        text-align: center;
        padding: 5px 10px;
        color: rgb(120,120,120);
        font-size: 9px;
        background: rgb(249,249,249);
        border-top: 1px solid rgb(238,238,238);
        margin-top: 5px;
    }}
</style>
</head>
<body>
    <div class='ticket-container'>
        <div class='ticket-header'>
            <h2>E-Ticket</h2>
        </div>
        <div class='ticket-body'>
            {logoHtml}
            <div class='ticket-number'>
                <h3>{_item.TicketNumber}</h3>
            </div>
            <div class='ticket-content'>
                <div class='ticket-info'>
                    <table class='info-table'>
                        <tr>
                            <td class='info-label'>Event Name:</td>
                            <td class='info-value'><b>{_item.EventName.ToUpper()}</b></td>
                        </tr>
                        <tr>
                            <td class='info-label'>Show Date:</td>
                            <td class='info-value'><b>{_item.ShowDateTime.ToUpper()}</b></td>
                        </tr>
                        <tr>
                            <td class='info-label'>Customer Name:</td>
                            <td class='info-value'><b>{_item.CustomerName.ToUpper()}</b></td>
                        </tr>
                        <tr>
                            <td class='info-label'>Phone Number:</td>
                            <td class='info-value'><b>{_item.CustomerPhone.ToUpper()}</b></td>
                        </tr>
                        <tr>
                            <td class='info-label'>Reserved Seats: ({seatLabelsList.Count})</td>
                            <td class='info-value'><span class='seat_category'>{_item.TicketCategory} - </span>{seats}</td>
                        </tr>
                    </table>
                </div>
                {imageHtml}
            </div>
        </div>
        <div class='footer'>
            Thank you for your reservation. Please present this ticket at the venue.
        </div>
    </div>
</body>
</html>";
                var _ticketTitle = _item.UpdatedDateTime.HasValue
                   ? $"E-TICKET_{_item.TicketNumber}_UPDATED"
                   : $"E-TICKET_{_item.TicketNumber}";
                var pdfDoc = new HtmlToPdfDocument()
                {
                    GlobalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = new PechkinPaperSize(_docWidth, _docHeight),
                        Margins = new MarginSettings { Top = 0, Bottom = 0, Left = 0, Right = 0 }
                    },
                    Objects = {
                    new ObjectSettings()
                    {
                        PagesCount = true,
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8", LoadImages = true }
                    }
                }
                };

                byte[] pdf = _converter.Convert(pdfDoc);
                return File(pdf, "application/pdf", $"{_ticketTitle}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetPDF in {_Title} Controller due to {ex}");
            }
            TempData["Message"] = "0,Error while generating PDF";
            return RedirectToAction("Details", "Event", new { ID = EventID });
        }
        private string GetImageSource(string imageName, string relativePath)
        {
            try
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath, imageName);

                if (!System.IO.File.Exists(imagePath))
                {
                    _logger.LogWarning($"Image not found: {imagePath}");
                    return string.Empty;
                }
                // Option 2: Base64 (more reliable)
                byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
                string extension = Path.GetExtension(imageName).TrimStart('.').ToLower();
                return $"data:image/{extension};base64,{Convert.ToBase64String(imageBytes)}";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading image {imageName}: {ex.Message}");
                return string.Empty;
            }
        }
        //Close Reservation
        [HttpPost]
        public async Task<IActionResult> Close(int ID, int EventID)
        {
            var result = await _reservationService.CloseReservationAsync(ID, EventID, _UserID);
            TempData["Message"] = (result.Success ? "1," : "0,") + result.Message;
            return RedirectToAction("Details", "Event", new { ID = EventID });
        }

    }
}
