using DinkToPdf;
using DinkToPdf.Contracts;
using EFCore.BulkExtensions;
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
        private readonly string _Title = "Reservation";
        private readonly int _UserID;
        public ReservationController(AppDbContext context, IHttpContextAccessor httpContextAccessor,IConverter converter,
            IWebHostEnvironment webHostEnvironment, ILogger<ReservationController> logger,
            IReservationService reservationService, IShowService showService,
            ITicketService ticketService, IPaymentModeService paymentModeService,
            IReservationStatusService reservationStatusService, IUserService userService)
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
                    var _item = new Reservation();
                    if (Model.AmountRemaining == 0)
                    {
                        Model.IsFullyPaid = true;
                    }
                    var _reservationStatusTitle = _context.ReservationStatuses.Where(x => !x.IsDeleted
                     && x.ID == Model.ReservationStatusID).Select(x => x.Title).FirstOrDefault();
                    if (Model.ID != 0)
                    {
                        var isExists = _context.ReservationTickets.Where(x => x.ReservationID != Model.ID && Model.TicketIDs.Contains(x.TicketID))
                         .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,Invalid Tickets Selected";
                            return RedirectToAction("AddOrEdit", _Title, new { ID = Model.ID, EventID = Model.EventID });
                        }
                        _item = _context.Reservations.Where(x => !x.IsDeleted && x.ID == Model.ID)
                            .AsNoTracking().FirstOrDefault();
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
                                _item.ActionDateTime = DateTime.UtcNow.AddHours(5);
                            }
                            if (!string.IsNullOrEmpty(_reservationStatusTitle) && _reservationStatusTitle.ToLower() == "closed")
                            {
                                _item.ClosedByUserID = _UserID;
                                _item.ClosingDateTime = DateTime.UtcNow.AddHours(5);
                            }
                            _item.ShowID = Model.ShowID;
                            _item.PaymentModeID = Model.PaymentModeID;
                            _item.ReservationStatusID = Model.ReservationStatusID;
                            _item.UpdatedByUserID = _UserID;
                            _item.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                            _context.Entry(_item).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                            var _existingRsvs = _context.ReservationTickets
                                .Where(x => x.ReservationID == Model.ID).ToList();
                            if (_existingRsvs != null && _existingRsvs.Count > 0)
                            {
                                List<int> _existingTicketIDs = _existingRsvs.Select(x => x.TicketID).ToList();
                                bool _isSameList = Model.TicketIDs.SequenceEqual(_existingTicketIDs);
                                if (!_isSameList)
                                {
                                    await _context.Tickets
                                    .Where(x => !x.IsDeleted && _existingTicketIDs.Contains(x.ID))
                                    .BatchUpdateAsync(x => new Ticket
                                    {
                                        CostPrice = x.CostPrice,
                                        IsReserved = false,
                                        ReservationDateTime = null,
                                        IsRefunded = x.IsRefunded,
                                        RefundDateTime = x.RefundDateTime,
                                        IsCancelled = x.IsCancelled,
                                        CancellationDateTime = x.CancellationDateTime,
                                        SalePrice = 0,
                                        Discount = 0,
                                        UpdatedByUserID = _UserID,
                                        UpdatedDateTime = DateTime.UtcNow.AddHours(5),
                                    });
                                    _context.ReservationTickets.RemoveRange(_existingRsvs);
                                }
                            }
                            TempData["Message"] = $"1,{_Title} Successfully Updated";
                        }
                    }
                    else
                    {
                        var isExists = _context.ReservationTickets.Where(x => Model.TicketIDs.Contains(x.TicketID))
                        .Any();
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
                            Model.BookingDateTime = DateTime.UtcNow.AddHours(5);
                        }
                        _item.IsApproved = Model.IsApproved;
                        if (_item.IsApproved.HasValue)
                        {
                            _item.ActionByUserID = Model.ActionByUserID.HasValue ? Model.ActionByUserID : _UserID;
                            _item.ActionDateTime = DateTime.UtcNow.AddHours(5);
                        }
                        if (!string.IsNullOrEmpty(_reservationStatusTitle) && _reservationStatusTitle.ToLower() == "closed")
                        {
                            _item.ClosedByUserID = _UserID;
                            _item.ClosingDateTime = DateTime.UtcNow.AddHours(5);
                        }
                        _item.BookingDateTime = Model.BookingDateTime;
                        _item.ShowID = Model.ShowID;
                        _item.PaymentModeID = Model.PaymentModeID;
                        _item.ReservationStatusID = Model.ReservationStatusID;
                        _item.CreatedByUserID = _UserID;
                        _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.Reservations.Add(_item);
                        await _context.SaveChangesAsync();
                        TempData["Message"] = $"1,{_Title} Successfully Added";
                    }

                    if (_item != null && Model.ImageFile != null)
                    {
                        var _eventName = _context.Events.Where(x => !x.IsDeleted && x.ID == Model.EventID).AsNoTracking().Select(x => x.Title).FirstOrDefault();
                        if (!string.IsNullOrEmpty(_eventName))
                        {
                            if (_eventName.Contains(" "))
                            {
                                _eventName = _eventName.Replace(" ", "");
                            }
                        }
                        var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "reservations", _eventName);
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        // Build initial filename
                        var baseFileName = $"payment_slip_{_item.TicketNumber}";

                        if (!string.IsNullOrEmpty(_reservationStatusTitle))
                        {
                            if (_reservationStatusTitle.Contains(" "))
                            {
                                _reservationStatusTitle = _reservationStatusTitle.Replace(" ", "");
                            }
                            baseFileName = $"payment_slip_{_item.TicketNumber}_{_reservationStatusTitle}";
                        }
                        var extension = Path.GetExtension(Model.ImageFile.FileName);
                        var _fileName = baseFileName + extension;
                        var _filePath = Path.Combine(folderPath, _fileName);
                        // Ensure unique filename if file exists
                        int counter = 1;
                        while (System.IO.File.Exists(_filePath))
                        {
                            _fileName = $"{baseFileName}_{counter}{extension}";
                            _filePath = Path.Combine(folderPath, _fileName);
                            counter++;
                        }
                        // Save file
                        var compressedBytes = await AppHelper.CompressImageAsync(Model.ImageFile);
                        await System.IO.File.WriteAllBytesAsync(_filePath, compressedBytes);
                        _context.ReservationImages.Add(new ReservationImage()
                        {
                            ReservationID = _item.ID,
                            ImagePath = _fileName,
                            CreatedByUserID = _UserID,
                            CreatedDateTime = DateTime.UtcNow.AddHours(5),
                        });
                        await _context.SaveChangesAsync();
                    }
                    if (_item != null && Model.TicketIDs != null && Model.TicketIDs.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(_reservationStatusTitle))
                        {
                            var _existingTicketsPerReservation = _context.ReservationTickets.Where(x => x.ReservationID == _item.ID).ToList();
                            if (_reservationStatusTitle.ToLower() == "arrivalpending" || _reservationStatusTitle.ToLower() == "arrival pending")
                            {
                                if (_existingTicketsPerReservation.Count > 0)
                                {
                                    _context.ReservationTickets.RemoveRange(_existingTicketsPerReservation);
                                    await _context.SaveChangesAsync();
                                }
                                foreach (var _ticketID in Model.TicketIDs)
                                {
                                    var _isExists = _context.ReservationTickets.Where(x => x.TicketID == _ticketID
                                    && x.ReservationID == _item.ID).Any();
                                    if (!_isExists)
                                    {
                                        _context.ReservationTickets.Add(new ReservationTicket()
                                        {
                                            ReservationID = _item.ID,
                                            TicketID = _ticketID,
                                            CreatedByUserID = _UserID,
                                            CreatedDateTime = DateTime.UtcNow.AddHours(5)
                                        });
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                await _context.SaveChangesAsync();
                            }
                            if (_item.IsApproved.HasValue && _item.IsApproved.Value)
                            {
                                if (_reservationStatusTitle.ToLower() == "arrivalpending" || _reservationStatusTitle.ToLower() == "arrival pending")
                                {
                                    await _context.Tickets
                               .Where(x => !x.IsDeleted && Model.TicketIDs.Contains(x.ID))
                               .BatchUpdateAsync(x => new Ticket
                               {
                                   CostPrice = _item.CostPrice / Model.TicketIDs.Count,
                                   IsReserved = true,
                                   IsRefunded = x.IsRefunded,
                                   RefundDateTime = x.RefundDateTime,
                                   IsCancelled = x.IsCancelled,
                                   CancellationDateTime = x.CancellationDateTime,
                                   ReservationDateTime = DateTime.UtcNow.AddHours(5),
                                   SalePrice = _item.SalePrice / Model.TicketIDs.Count,
                                   Discount = _item.Discount / Model.TicketIDs.Count,
                                   UpdatedByUserID = _UserID,
                                   UpdatedDateTime = DateTime.UtcNow.AddHours(5),
                               });
                                }
                                else if (_reservationStatusTitle.ToLower() == "refunded")
                                {
                                    await _context.Tickets
                                .Where(x => !x.IsDeleted && Model.TicketIDs.Contains(x.ID))
                                .BatchUpdateAsync(x => new Ticket
                                {
                                    CostPrice = _item.CostPrice / Model.TicketIDs.Count,
                                    IsReserved = x.IsReserved,
                                    IsRefunded = true,
                                    RefundDateTime = DateTime.UtcNow.AddHours(5),
                                    IsCancelled = x.IsCancelled,
                                    CancellationDateTime = x.CancellationDateTime,
                                    ReservationDateTime = x.ReservationDateTime,
                                    SalePrice = _item.SalePrice / Model.TicketIDs.Count,
                                    Discount = _item.Discount / Model.TicketIDs.Count,
                                    UpdatedByUserID = _UserID,
                                    UpdatedDateTime = DateTime.UtcNow.AddHours(5),
                                });
                                }
                                else if (_reservationStatusTitle.ToLower() == "cancelled")
                                {
                                    if (_existingTicketsPerReservation.Count > 0)
                                    {
                                        await _context.Tickets
                                .Where(x => !x.IsDeleted && _existingTicketsPerReservation.Select(x => x.TicketID).Contains(x.ID))
                                .BatchUpdateAsync(x => new Ticket
                                {
                                    CostPrice = x.CostPrice,
                                    IsReserved = false,
                                    IsRefunded = x.IsRefunded,
                                    RefundDateTime = x.RefundDateTime,
                                    ReservationDateTime = null,
                                    SalePrice = x.SalePrice,
                                    Discount = x.Discount,
                                    UpdatedByUserID = _UserID,
                                    UpdatedDateTime = DateTime.UtcNow.AddHours(5),
                                });
                                        _context.ReservationTickets.RemoveRange(_existingTicketsPerReservation);
                                    }
                                    foreach (var _ticketID in Model.TicketIDs)
                                    {
                                        var _isExists = _context.ReservationTickets.Where(x => x.TicketID == _ticketID
                                        && x.ReservationID == _item.ID).Any();
                                        if (!_isExists)
                                        {
                                            _context.ReservationTickets.Add(new ReservationTicket()
                                            {
                                                ReservationID = _item.ID,
                                                TicketID = _ticketID,
                                                CreatedByUserID = _UserID,
                                                CreatedDateTime = DateTime.UtcNow.AddHours(5)
                                            });
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    await _context.Tickets
                                .Where(x => !x.IsDeleted && Model.TicketIDs.Contains(x.ID))
                                .BatchUpdateAsync(x => new Ticket
                                {
                                    CostPrice = x.CostPrice,
                                    IsReserved = false,
                                    IsRefunded = x.IsRefunded,
                                    RefundDateTime = x.RefundDateTime,
                                    ReservationDateTime = null,
                                    SalePrice = x.SalePrice,
                                    Discount = x.Discount,
                                    UpdatedByUserID = _UserID,
                                    UpdatedDateTime = DateTime.UtcNow.AddHours(5),
                                });
                                }
                            }

                            await _context.SaveChangesAsync();
                        }
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

                if (ID != 0)
                    _item = _reservationService.GetDetailsByID(ID);
                string _docWidth = "544mm";
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
        margin: 0;
        padding: 0;
        font-family: 'Poppins', sans-serif;
        background:  rgb(255,255,255);
    }}
.ticket-container {{
        width: {_docWidth};
        height: {_docHeight};
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
        width: 48%;
        vertical-align: top;
    }}
    .image_info {{
        display: inline-block;
        width: 48%;
        vertical-align: top;
        text-align: right;
    }}
    .image-info img {{
        border-radius: 10px;
        max-width: 100%;
        height: auto;
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
                    <div class='info-row'><div class='info-label'>Event Name:</div><div class='info-value'>{_item.EventName}</div></div>
                    <div class='info-row'><div class='info-label'>Show Date:</div><div class='info-value'>{_item.ShowDateTime}</div></div>
                    <div class='info-row'><div class='info-label'>Customer Name:</div><div class='info-value'>{_item.CustomerName}</div></div>
                    <div class='info-row'><div class='info-label'>Phone Number:</div><div class='info-value'>{_item.CustomerPhone}</div></div>
                    <div class='info-row'>
                        <div class='info-label'>Reserved Seats: ({_item.SeatLabels.Split(',').Count()})</div>
                        <div class='info-value'>{string.Join("", _item.SeatLabels.Split(',').Select(s => $"<span class='seats-badge'>{s.Trim()}</span>"))}</div>
                    </div>
                </div>
                <div class='image_info'>
                    <img src='{eventImageSrc}'/>
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
                        Margins = new MarginSettings { Top = 5, Bottom = 5,Left= 5, Right= 5 }
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

    }
}
