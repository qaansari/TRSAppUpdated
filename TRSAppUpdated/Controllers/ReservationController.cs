using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using Syncfusion.EJ2.FileManager.Base;
using Syncfusion.EJ2.TreeGrid;
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
        public IActionResult AddOrEdit(int ID = 0, int EventID = 0)
        {

            var _item = new ReservationVM();
            try
            {
                ViewData["TicketList"] = _ticketService.GetTicketsByEventID(EventID);
                ViewData["ShowList"] = _showService.GetActiveShowsByEventID(EventID);
                ViewData["PaymentModeList"] = _paymentModeService.GetAll();
                ViewData["ReservationStatusList"] = _reservationStatusService.GetAll();
                ViewData["UserList"] = _userService.GetAll();
                if (EventID != 0)
                {
                    ViewBag.EventID = EventID;
                }
                if (ID != 0)
                {
                    _item = _context.Reservations
                        .Include(x => x.ReservationTickets)
                        .Where(x => !x.IsDeleted && x.ID == ID)
                                  .Select(x => new ReservationVM()
                                  {
                                      ID = x.ID,
                                      TicketNumber = x.TicketNumber,
                                      EventID = EventID,
                                      CustomerName = x.CustomerName,
                                      CustomerPhone = x.CustomerPhone,
                                      TicketIDs = x.ReservationTickets.Select(x => x.TicketID).ToList(),
                                      CostPrice = x.CostPrice,
                                      Discount = x.Discount,
                                      SalePrice = x.SalePrice,
                                      AmountPaid = x.AmountPaid,
                                      AmountRemaining = x.AmountRemaining,
                                      IsFullyPaid = x.IsFullyPaid,
                                      IsApproved = x.IsApproved,
                                      ActionByUserID = x.ActionByUserID,
                                      ActionDateTime = x.ActionDateTime,
                                      ClosedByUserID = x.ClosedByUserID,
                                      ClosingDateTime = x.ClosingDateTime,
                                      TransactionID = x.TransactionID,
                                      BookingDateTime = x.BookingDateTime,
                                      ShowID = x.ShowID,
                                      PaymentModeID = x.PaymentModeID,
                                      ReservationStatusID = x.ReservationStatusID,
                                      CreatedByUserID = x.CreatedByUserID,
                                      CreatedDateTime = x.CreatedDateTime,
                                  })
                                  .AsNoTracking().FirstOrDefault();
                }
                else
                {
                    _item.EventID = EventID;
                    _item.TicketNumber = (Guid.NewGuid().ToString().Substring(0, 7)).ToUpper();
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error occured at AddOrEdit (GET) in {_Title} Controller due to {ex}");
            }
            return View(_item);
        }
        [Authorize(Roles = "Super Admin,Admin")]
        //AddOrEdit (POST)
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(ReservationVM Model)
        {
            try
            {
                ViewData["TicketList"] = _ticketService.GetTicketsByEventID(Model.EventID);
                ViewData["ShowList"] = _showService.GetActiveShowsByEventID(Model.EventID);
                ViewData["PaymentModeList"] = _paymentModeService.GetAll();
                ViewData["ReservationStatusList"] = _reservationStatusService.GetAll();
                ViewData["UserList"] = _userService.GetAll();
                if (ModelState.IsValid)
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        var now = DateTime.UtcNow.AddHours(5);
                        var _item = new Reservation();
                        if (Model.AmountRemaining == 0)
                        {
                            Model.IsFullyPaid = true;
                        }
                        var _reservationStatusTitle = await _context.ReservationStatuses
                            .Where(x => !x.IsDeleted && x.ID == Model.ReservationStatusID)
                            .Select(x => x.Title)
                            .FirstOrDefaultAsync();

                        if (Model.ID != 0)
                        {
                            var isExists = await _context.ReservationTickets
                                .Where(x => x.ReservationID != Model.ID && Model.TicketIDs.Contains(x.TicketID))
                                .AnyAsync();
                            if (isExists)
                            {
                                TempData["Message"] = $"0,Invalid Tickets Selected";
                                return RedirectToAction("AddOrEdit", _Title, new { ID = Model.ID, EventID = Model.EventID });
                            }
                            _item = await _context.Reservations
                                .Where(x => !x.IsDeleted && x.ID == Model.ID)
                                .AsNoTracking()
                                .FirstOrDefaultAsync();

                            if (_item != null)
                            {
                                _item.CustomerName = Model.CustomerName.Trim();
                                _item.CustomerPhone = Model.CustomerPhone.Trim();
                                _item.CostPrice = Model.CostPrice;
                                _item.Discount = Model.Discount;
                                _item.SalePrice = Model.SalePrice;
                                _item.AmountPaid = Model.AmountPaid;
                                _item.AmountRemaining = Model.AmountRemaining;
                                _item.IsFullyPaid = Model.IsFullyPaid;
                                _item.TransactionID = Model.TransactionID;
                                _item.BookingDateTime = Model.BookingDateTime;
                                _item.IsApproved = Model.IsApproved;
                                if (_item.IsApproved.HasValue)
                                {
                                    _item.ActionByUserID = Model.ActionByUserID.HasValue ? Model.ActionByUserID : _UserID;
                                    _item.ActionDateTime = now;
                                }
                                if (!string.IsNullOrEmpty(_reservationStatusTitle) && _reservationStatusTitle.Equals("closed", StringComparison.OrdinalIgnoreCase))
                                {
                                    _item.ClosedByUserID = _UserID;
                                    _item.ClosingDateTime = now;
                                }
                                _item.ShowID = Model.ShowID;
                                _item.PaymentModeID = Model.PaymentModeID;
                                _item.ReservationStatusID = Model.ReservationStatusID;
                                _item.UpdatedByUserID = _UserID;
                                _item.UpdatedDateTime = now;
                                _context.Entry(_item).State = EntityState.Modified;
                                await _context.SaveChangesAsync();

                                var _existingRsvs = await _context.ReservationTickets
                                    .Where(x => x.ReservationID == Model.ID).ToListAsync();
                                if (_existingRsvs != null && _existingRsvs.Count > 0)
                                {
                                    List<int> _existingTicketIDs = _existingRsvs.Select(x => x.TicketID).ToList();
                                    bool _isSameList = Model.TicketIDs.SequenceEqual(_existingTicketIDs);
                                    if (!_isSameList)
                                    {
                                        await _context.Tickets
                                            .Where(x => !x.IsDeleted && _existingTicketIDs.Contains(x.ID))
                                            .ExecuteUpdateAsync(s => s
                                                .SetProperty(t => t.IsReserved, false)
                                                .SetProperty(t => t.ReservationDateTime, (DateTime?)null)
                                                .SetProperty(t => t.SalePrice, 0)
                                                .SetProperty(t => t.Discount, 0)
                                                .SetProperty(t => t.UpdatedByUserID, _UserID)
                                                .SetProperty(t => t.UpdatedDateTime, now));

                                        _context.ReservationTickets.RemoveRange(_existingRsvs);
                                    }
                                }
                                TempData["Message"] = $"1,{_Title} Successfully Updated";
                            }
                        }
                        else
                        {
                            var isExists = await _context.ReservationTickets
                                .Where(x => Model.TicketIDs.Contains(x.TicketID))
                                .AnyAsync();
                            if (isExists)
                            {
                                TempData["Message"] = $"0,Invalid Tickets Selected";
                                return RedirectToAction("AddOrEdit", _Title, new { ID = Model.ID, EventID = Model.EventID });
                            }
                            _item.TicketNumber = Model.TicketNumber.Trim();
                            _item.CustomerName = Model.CustomerName.Trim();
                            _item.CustomerPhone = Model.CustomerPhone.Trim();
                            _item.CostPrice = Model.CostPrice;
                            _item.Discount = Model.Discount;
                            _item.SalePrice = Model.SalePrice;
                            _item.AmountPaid = Model.AmountPaid;
                            _item.AmountRemaining = Model.AmountRemaining;
                            _item.IsFullyPaid = Model.IsFullyPaid;
                            _item.TransactionID = Model.TransactionID;
                            if (!Model.BookingDateTime.HasValue)
                            {
                                Model.BookingDateTime = now;
                            }
                            _item.IsApproved = Model.IsApproved;
                            if (_item.IsApproved.HasValue)
                            {
                                _item.ActionByUserID = Model.ActionByUserID.HasValue ? Model.ActionByUserID : _UserID;
                                _item.ActionDateTime = now;
                            }
                            if (!string.IsNullOrEmpty(_reservationStatusTitle) && _reservationStatusTitle.Equals("closed", StringComparison.OrdinalIgnoreCase))
                            {
                                _item.ClosedByUserID = _UserID;
                                _item.ClosingDateTime = now;
                            }
                            _item.BookingDateTime = Model.BookingDateTime;
                            _item.ShowID = Model.ShowID;
                            _item.PaymentModeID = Model.PaymentModeID;
                            _item.ReservationStatusID = Model.ReservationStatusID;
                            _item.CreatedByUserID = _UserID;
                            _item.CreatedDateTime = now;
                            _context.Reservations.Add(_item);
                            await _context.SaveChangesAsync();
                            TempData["Message"] = $"1,{_Title} Successfully Added";
                        }

                        if (_item != null && Model.ImageFile != null)
                        {
                            var _eventName = await _context.Events
                                .Where(x => !x.IsDeleted && x.ID == Model.EventID)
                                .AsNoTracking()
                                .Select(x => x.Title)
                                .FirstOrDefaultAsync();

                            if (!string.IsNullOrEmpty(_eventName))
                            {
                                _eventName = _eventName.Replace(" ", "");
                            }
                            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "reservations", _eventName ?? "default");
                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            var baseFileName = $"payment_slip_{_item.TicketNumber}";
                            if (!string.IsNullOrEmpty(_reservationStatusTitle))
                            {
                                baseFileName = $"payment_slip_{_item.TicketNumber}_{_reservationStatusTitle.Replace(" ", "")}";
                            }
                            var extension = Path.GetExtension(Model.ImageFile.FileName);
                            var _fileName = baseFileName + extension;
                            var _filePath = Path.Combine(folderPath, _fileName);
                            int counter = 1;
                            while (System.IO.File.Exists(_filePath))
                            {
                                _fileName = $"{baseFileName}_{counter}{extension}";
                                _filePath = Path.Combine(folderPath, _fileName);
                                counter++;
                            }
                            var compressedBytes = await AppHelper.CompressImageAsync(Model.ImageFile);
                            await System.IO.File.WriteAllBytesAsync(_filePath, compressedBytes);
                            _context.ReservationImages.Add(new ReservationImage()
                            {
                                ReservationID = _item.ID,
                                ImagePath = _fileName,
                                CreatedByUserID = _UserID,
                                CreatedDateTime = now,
                            });
                            await _context.SaveChangesAsync();
                        }

                        if (_item != null && Model.TicketIDs != null && Model.TicketIDs.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(_reservationStatusTitle))
                            {
                                var _existingTicketsPerReservation = await _context.ReservationTickets
                                    .Where(x => x.ReservationID == _item.ID).ToListAsync();
                                var statusLower = _reservationStatusTitle.ToLower();

                                if (statusLower == "arrivalpending" || statusLower == "arrival pending")
                                {
                                    if (_existingTicketsPerReservation.Count > 0)
                                    {
                                        _context.ReservationTickets.RemoveRange(_existingTicketsPerReservation);
                                        await _context.SaveChangesAsync();
                                    }
                                    foreach (var _ticketID in Model.TicketIDs)
                                    {
                                        var _isExists = await _context.ReservationTickets
                                            .Where(x => x.TicketID == _ticketID && x.ReservationID == _item.ID)
                                            .AnyAsync();
                                        if (!_isExists)
                                        {
                                            _context.ReservationTickets.Add(new ReservationTicket()
                                            {
                                                ReservationID = _item.ID,
                                                TicketID = _ticketID,
                                                CreatedByUserID = _UserID,
                                                CreatedDateTime = now
                                            });
                                        }
                                    }
                                    await _context.SaveChangesAsync();
                                }

                                if (_item.IsApproved.HasValue && _item.IsApproved.Value)
                                {
                                    double costPerTicket = Model.TicketIDs.Count > 0 ? (double)_item.CostPrice / Model.TicketIDs.Count : 0.0;
                                    double salePricePerTicket = Model.TicketIDs.Count > 0 ? (double)_item.SalePrice / Model.TicketIDs.Count : 0.0;
                                    double discountPerTicket = Model.TicketIDs.Count > 0 ? (double)_item.Discount / Model.TicketIDs.Count : 0.0;

                                    if (statusLower == "arrivalpending" || statusLower == "arrival pending")
                                    {
                                        await _context.Tickets
                                            .Where(x => !x.IsDeleted && Model.TicketIDs.Contains(x.ID))
                                            .ExecuteUpdateAsync(s => s
                                                .SetProperty(t => t.CostPrice, costPerTicket)
                                                .SetProperty(t => t.IsReserved, true)
                                                .SetProperty(t => t.ReservationDateTime, now)
                                                .SetProperty(t => t.SalePrice, salePricePerTicket)
                                                .SetProperty(t => t.Discount, discountPerTicket)
                                                .SetProperty(t => t.UpdatedByUserID, _UserID)
                                                .SetProperty(t => t.UpdatedDateTime, now));
                                    }
                                    else if (statusLower == "refunded")
                                    {
                                        await _context.Tickets
                                            .Where(x => !x.IsDeleted && Model.TicketIDs.Contains(x.ID))
                                            .ExecuteUpdateAsync(s => s
                                                .SetProperty(t => t.CostPrice, costPerTicket)
                                                .SetProperty(t => t.IsRefunded, true)
                                                .SetProperty(t => t.RefundDateTime, now)
                                                .SetProperty(t => t.SalePrice, salePricePerTicket)
                                                .SetProperty(t => t.Discount, discountPerTicket)
                                                .SetProperty(t => t.UpdatedByUserID, _UserID)
                                                .SetProperty(t => t.UpdatedDateTime, now));
                                    }
                                    else if (statusLower == "cancelled")
                                    {
                                        if (_existingTicketsPerReservation.Count > 0)
                                        {
                                            var existingTicketIds = _existingTicketsPerReservation.Select(x => x.TicketID).ToList();
                                            await _context.Tickets
                                                .Where(x => !x.IsDeleted && existingTicketIds.Contains(x.ID))
                                                .ExecuteUpdateAsync(s => s
                                                    .SetProperty(t => t.IsReserved, false)
                                                    .SetProperty(t => t.ReservationDateTime, (DateTime?)null)
                                                    .SetProperty(t => t.UpdatedByUserID, _UserID)
                                                    .SetProperty(t => t.UpdatedDateTime, now));

                                            _context.ReservationTickets.RemoveRange(_existingTicketsPerReservation);
                                        }
                                        foreach (var _ticketID in Model.TicketIDs)
                                        {
                                            var _isExists = await _context.ReservationTickets
                                                .Where(x => x.TicketID == _ticketID && x.ReservationID == _item.ID)
                                                .AnyAsync();
                                            if (!_isExists)
                                            {
                                                _context.ReservationTickets.Add(new ReservationTicket()
                                                {
                                                    ReservationID = _item.ID,
                                                    TicketID = _ticketID,
                                                    CreatedByUserID = _UserID,
                                                    CreatedDateTime = now
                                                });
                                            }
                                        }
                                        await _context.Tickets
                                            .Where(x => !x.IsDeleted && Model.TicketIDs.Contains(x.ID))
                                            .ExecuteUpdateAsync(s => s
                                                .SetProperty(t => t.IsReserved, false)
                                                .SetProperty(t => t.ReservationDateTime, (DateTime?)null)
                                                .SetProperty(t => t.UpdatedByUserID, _UserID)
                                                .SetProperty(t => t.UpdatedDateTime, now));
                                    }
                                }

                                await _context.SaveChangesAsync();
                            }
                        }

                        await transaction.CommitAsync();
                        _cacheService.Remove(CacheKeys.DashboardMetrics);
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
                return RedirectToAction("Details", "Event", new { ID = Model.EventID });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at AddOrEdit (POST) in {_Title} Controller due to {ex}");
            }
            return View(Model);
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
                string _docWidth = "490mm";
                string _docHeight = "218mm";
                string logoImageSrc = GetImageSource("web-logo-light.png", "assets/images");
                string eventImageSrc = GetImageSource(_item.EventImagePath, "assets/images/events");
                string html = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<style>
    
    body {{
        margin: 0 auto;
        padding: 0;
        font-family: 'Poppins', sans-serif;
        background:  rgb(255,255,255);
        width:{_docWidth};
        height:{_docHeight};
    }}
.ticket-container {{
        width: 100%;
        height: 100%;
        margin: 0 auto;
        background: rgb(255,255,255);
        border-radius: 10px;
        box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
    }}
    .ticket-header {{
        background: linear-gradient(135deg, rgb(26,164,139), rgb(69,165,121));
        color: #fff;
        padding: 20px;
        text-align: center;
    }}
    .ticket-header h2 {{
        margin: 0;
        font-size: 24px;
        font-weight: 600;
    }}
    .ticket-body {{
        padding: 15px;
    }}
    .ticket_heading {{
        text-align: center;
        text-transform: uppercase;
    }}
    .ticket_heading h2 {{
        font-size: 24px;
        color: rgb(26,164,139);
    }}
    .ticket-content {{
        display: inline-block;
        width: 100%;
        vertical-align: top;
    }}
    .ticket-info {{
        display: inline-block;
        width: 38%;
        vertical-align: top;
    }}
    .image_info {{
        display: inline-block;
        width: 60%;
        vertical-align: top;
        text-align: right;
    }}
    .event_img {{
        border-radius: 10px;
        width: 100%;
        height: 380px;
    }}
    .info-row {{
        display: flex;
        margin-bottom: 12px;
        padding-bottom: 12px;
        border-bottom: 1px solid rgb(238,238,238);
    }}
    .info-label {{
        font-weight: 600;
        width: 200px;
        color: rgb(19,7,22);
    }}
    .info-value {{
        flex: 1;
        color: rgb(10,3,3);
    }}
    .ticket-number {{
        background: rgb(248,249,250);
        padding: 15px;
        border-radius: 8px;
        text-align: center;
        margin: 20px 0;
        border: 1px dashed rgb(221,221,221);
    }}
    .ticket-number h3 {{
        margin: 0;
        font-size: 28px;
        color: rgb(26,164,139);
        letter-spacing: 2px;
        text-transform: uppercase;
    }}
    .logo_img_section {{
        text-align: center;
    }}
    .logo_img {{
        width: 200px;
        height: auto;
    }}
    .footer {{
        text-align: center;
        padding: 15px;
        color: rgb(153,153,153);
        font-size: 12px;
        background: rgb(249,249,249);
        border-top: 1px solid rgb(238,238,238);
    }}
    .seats-badge {{
        display: inline-block;
        background: rgb(88,255,217);
        color: rgb(16,12,12);
        padding: 5px 10px;
        border-radius: 5px;
        font-weight: 600;
        margin-right: 5px;
        margin-bottom: 5px;
    }}
.seat_category{{
    font-weight: 600;
    text-transform: uppercase;
}}
</style>
</head>
<body>
    <div class='ticket-container'>
        <div class='ticket-header'>
            <h2>E-Ticket</h2>
        </div>
        <div class='ticket-body'>
            <div class='logo_img_section'>
               <img src='{logoImageSrc}' class=""logo_img"" />
            </div>
            <div class='ticket_heading'>
                <h2>E-Ticket</h2>
            </div>
            <div class='ticket-number'>
                <h3>{_item.TicketNumber}</h3>
            </div>
            <div class='ticket-content'>
                <div class='ticket-info'>
                    <div class='info-row'><div class='info-label'>Event Name:</div><div class='info-value'><b>{_item.EventName.ToUpper()}</b></div></div>
                    <div class='info-row'><div class='info-label'>Show Date:</div><div class='info-value'><b>{_item.ShowDateTime.ToUpper()}</b></div></div>
                    <div class='info-row'><div class='info-label'>Customer Name:</div><div class='info-value'><b>{_item.CustomerName.ToUpper()}</b></div></div>
                    <div class='info-row'><div class='info-label'>Phone Number:</div><div class='info-value'><b>{_item.CustomerPhone.ToUpper()}</b></div></div>
                    <div class='info-row'>
                        <div class='info-label'>Reserved Seats: ({(isGenEvent ? "General" : seatLabelsList.Count.ToString())})</div>
                        <div class='info-value'><span class='seat_category'>{_item.TicketCategory} - </span>{seats}</div>
                    </div>
                </div>
                <div class='image_info'>
                    <img src='{eventImageSrc}' class='event_img'/>
                </div>
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
                        Margins = new MarginSettings { Top = 5, Bottom = 5, Left = 5, Right = 5 }
                    },
                    Objects = {
                    new ObjectSettings()
                    {
                        PagesCount = true,
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8", LoadImages = true },
                        HeaderSettings = { FontSize = 9, Right = "" }
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
            try
            {
                var _item = _context.Reservations.Where(x => !x.IsDeleted && x.ID == ID && x.ReservationStatusID != 3)
                                  .AsNoTracking().FirstOrDefault();
                if (_item != null)
                {
                    _item.ReservationStatusID = 3;
                    _item.ClosedByUserID = _UserID;
                    _item.ClosingDateTime = DateTime.UtcNow.AddHours(5);
                    _context.ChangeTracker.Clear();
                    _context.Entry(_item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    _cacheService.Remove(CacheKeys.DashboardMetrics);
                    TempData["Message"] = $"1,{_Title} Successfully Closed";
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
            return RedirectToAction("Details", "Event", new { ID = EventID });
        }

    }
}
