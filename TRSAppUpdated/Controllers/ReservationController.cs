using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Syncfusion.EJ2.Layouts;
using System.Security.Claims;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.Services;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ReservationController> _logger;
        private readonly IReservationService _reservationService;
        private readonly IShowService _showService;
        private readonly ITicketService _ticketService;
        private readonly IPaymentModeService _paymentModeService;
        private readonly IReservationStatusService _reservationStatusService;
        private readonly string _Title = "Reservation";
        private readonly int _UserID;
        public ReservationController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment, ILogger<ReservationController> logger,
            IReservationService reservationService, IShowService showService,
            ITicketService ticketService, IPaymentModeService paymentModeService,
            IReservationStatusService reservationStatusService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _reservationService = reservationService;
            _showService = showService;
            _ticketService = ticketService;
            _paymentModeService = paymentModeService;
            _reservationStatusService = reservationStatusService;
            _logger = logger;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
        }
        [Authorize(Roles = "Super Admin,Admin")]
        public IActionResult AddOrEdit(int ID = 0, int EventID = 0)
        {

            var _item = new ReservationVM();
            try
            {
                ViewData["TicketList"] = _ticketService.GetTicketsByEventID(EventID);
                ViewData["ShowList"] = _showService.GeActiveShowsByEventID(EventID);
                ViewData["PaymentModeList"] = _paymentModeService.GetAll();
                ViewData["ReservationStatusList"] = _reservationStatusService.GetAll();
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
        //AddOrEdit (POST)
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(ReservationVM Model)
        {
            try
            {
                ViewData["TicketList"] = _ticketService.GetTicketsByEventID(Model.EventID);
                ViewData["ShowList"] = _showService.GeActiveShowsByEventID(Model.EventID);
                ViewData["PaymentModeList"] = _paymentModeService.GetAll();
                ViewData["ReservationStatusList"] = _reservationStatusService.GetAll();
                if (ModelState.IsValid)
                {
                    var _item = new Reservation();
                    if (Model.AmountRemaining == 0)
                    {
                        Model.IsFullyPaid = true;
                    }
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
                        var _reservationStatusTitle = _context.ReservationStatuses.Where(x => !x.IsDeleted
                        && x.ID == Model.ReservationStatusID).Select(x => x.Title).FirstOrDefault();
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
                        using (var stream = new FileStream(_filePath, FileMode.Create))
                        {
                            await Model.ImageFile.CopyToAsync(stream);
                        }
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
                        var _reservationStatusTitle = _context.ReservationStatuses.Where(x => !x.IsDeleted
                        && x.ID == Model.ReservationStatusID).Select(x => x.Title).FirstOrDefault();
                        if (!string.IsNullOrEmpty(_reservationStatusTitle))
                        {
                            if (_reservationStatusTitle.ToLower() == "arrival pending")
                            {

                                foreach (var _ticketID in Model.TicketIDs)
                                {
                                    _context.ReservationTickets.Add(new ReservationTicket()
                                    {
                                        ReservationID = _item.ReservationStatusID,
                                        TicketID = _ticketID,
                                        CreatedByUserID = _UserID,
                                        CreatedDateTime = DateTime.UtcNow.AddHours(5)
                                    });
                                }
                                await _context.SaveChangesAsync();
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
                            await _context.SaveChangesAsync();
                        }
                    }
                    return RedirectToAction("Details", "Event", new { ID = Model.EventID });
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error occured at AddOrEdit (POST) in {_Title} Controller due to {ex}");
            }
            return View(Model);
        }
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
    }
}
