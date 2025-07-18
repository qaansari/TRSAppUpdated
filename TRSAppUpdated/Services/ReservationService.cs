using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReservationService> _logger;
        private readonly string _Title = "Reservation";
        public ReservationService(AppDbContext context, ILogger<ReservationService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public ReservationDetailVM GetDetailsByID(int ID)
        {
            var _VM = new ReservationDetailVM();
            try
            {
                _VM = _context.Reservations.Where(x => x.ID == ID)
                    .AsNoTracking().Select(x => new ReservationDetailVM()
                    {
                        ID = x.ID,
                        EventName = x.Show.Event.Title,
                        TicketNumber = x.TicketNumber,
                        CustomerName = x.CustomerName,
                        CustomerPhone = x.CustomerPhone,
                        ShowDateTime = x.Show.Date.ToString("dddd, dd MMMM yyyy") + " "
                        + DateTime.Today.Add(x.Show.Time).ToString("hh:mm tt"),
                        PaymentModeTitle = x.PaymentMode.Title,
                        ReservationStatusTitle = x.PaymentMode.Title,
                        IsFullyPaid = x.IsFullyPaid,
                        CostPrice=x.CostPrice,
                        Discount=x.Discount,
                        SalePrice = x.SalePrice,
                        AmountPaid = x.AmountPaid,
                        ReservationImages= String.Join(",", (x.ReservationImages.Select(x => x.ImagePath).ToList())),
                        AmountRemaining = x.AmountRemaining,
                        BookingDateTime = x.BookingDateTime,
                        CreatedDateTime = x.CreatedDateTime,
                        UpdatedDateTime = x.UpdatedDateTime,
                        SeatLabels = String.Join(",", x.ReservationTickets.Select(x => x.Ticket.Seat.Column.Row.RowLabel + x.Ticket.Seat.SeatNumber).ToList()),
                    }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetDetailsByID in {_Title} Service due to {ex}");
            }
            return _VM;
        }
    }
}
