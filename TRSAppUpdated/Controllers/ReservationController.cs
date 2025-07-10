using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Layouts;
using System.Security.Claims;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ReservationController> _logger;
        private readonly IShowService _showService;
        private readonly ITicketService _ticketService;
        private readonly IPaymentModeService _paymentModeService;
        private readonly IReservationStatusService _reservationStatusService;
        private readonly string _Title = "Event";
        private readonly int _UserID;
        public ReservationController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment, ILogger<ReservationController> logger,
            IShowService showService, ITicketService ticketService,
            IPaymentModeService paymentModeService,IReservationStatusService reservationStatusService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _showService = showService;
            _ticketService = ticketService;
            _paymentModeService = paymentModeService;
            _reservationStatusService = reservationStatusService;
            _logger = logger;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
        }
        [Authorize(Roles ="Super Admin,Admin")]
        public IActionResult AddOrEdit(int ID = 0, int EventID = 0)
        {
           
            var _item = new ReservationVM();
            try
            {
                ViewData["TicketList"] = _ticketService.GetTicketsByEventID(EventID);
                ViewData["ShowList"] = _showService.GetShowsByEventID(EventID);
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
                    _item.TicketNumber= (Guid.NewGuid().ToString().Substring(0, 7)).ToUpper();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }
    }
}
