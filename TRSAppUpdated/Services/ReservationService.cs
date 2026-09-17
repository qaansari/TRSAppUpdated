using Microsoft.EntityFrameworkCore;
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
            try
            {
                var r = _context.Reservations
                    .Include(x => x.Show).ThenInclude(s => s.Event)
                    .Include(x => x.PaymentMode)
                    .Include(x => x.ReservationStatus)
                    .Include(x => x.ClosedByUser)
                    .Include(x => x.ReservationImages)
                    .Include(x => x.ReservationTickets)
                        .ThenInclude(rt => rt.Ticket)
                        .ThenInclude(t => t.Seat)
                        .ThenInclude(s => s.Column)
                        .ThenInclude(c => c.Row)
                    .AsNoTracking()
                    .FirstOrDefault(x => x.ID == ID);

                if (r == null)
                {
                    return new ReservationDetailVM();
                }

                var firstTicket = r.ReservationTickets?.FirstOrDefault()?.Ticket;
                var seatList = r.ReservationTickets?
                    .Where(rt => rt.Ticket?.Seat?.Column?.Row != null)
                    .Select(rt => $"{rt.Ticket.Seat.Column.Row.RowLabel}{rt.Ticket.Seat.SeatNumber}")
                    .ToList() ?? new List<string>();

                var imageList = r.ReservationImages?
                    .Select(ri => ri.ImagePath)
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToList() ?? new List<string>();

                string showDateTimeStr = string.Empty;
                if (r.Show != null)
                {
                    showDateTimeStr = r.Show.Date.ToString("dddd, dd MMMM yyyy") + " "
                        + DateTime.Today.Add(r.Show.Time).ToString("hh:mm tt");
                }

                return new ReservationDetailVM
                {
                    ID = r.ID,
                    EventName = r.Show?.Event?.Title ?? string.Empty,
                    TicketNumber = r.TicketNumber,
                    CustomerName = r.CustomerName,
                    CustomerPhone = r.CustomerPhone,
                    ShowDateTime = showDateTimeStr,
                    PaymentModeTitle = r.PaymentMode?.Title ?? string.Empty,
                    ReservationStatusTitle = r.ReservationStatus?.Title ?? string.Empty,
                    EventImagePath = r.Show?.Event?.ImagePath ?? string.Empty,
                    IsFullyPaid = r.IsFullyPaid,
                    TicketCategory = firstTicket?.Category ?? string.Empty,
                    CostPrice = r.CostPrice,
                    Discount = r.Discount,
                    SalePrice = r.SalePrice,
                    AmountPaid = r.AmountPaid,
                    ReservationImages = string.Join(",", imageList),
                    AmountRemaining = r.AmountRemaining,
                    BookingDateTime = r.BookingDateTime,
                    TransactionID = r.TransactionID,
                    CreatedDateTime = r.CreatedDateTime,
                    UpdatedDateTime = r.UpdatedDateTime,
                    ClosedByUserName = r.ClosedByUser != null ? r.ClosedByUser.FullName : string.Empty,
                    ClosedDateTime = r.ClosingDateTime.HasValue ? r.ClosingDateTime.Value.ToString("dddd, dd MMMM yyyy hh:mm tt") : string.Empty,
                    SeatLabels = string.Join(",", seatList)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetDetailsByID in {_Title} Service due to {ex}");
            }
            return new ReservationDetailVM();
        }
    }
}
