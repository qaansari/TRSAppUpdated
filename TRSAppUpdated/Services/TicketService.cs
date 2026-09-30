using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TicketService> _logger;
        private readonly string _Title = "Ticket";
        public TicketService(AppDbContext context, ILogger<TicketService> logger)
        {
            _context = context;
            _logger = logger;
        }
        //Get Tickets By EventID
        public List<TicketVM> GetTicketsByEventID(int EventID)
        {
            try
            {
                var _ticketList = _context.Tickets
                    .Where(x => !x.IsDeleted && !x.Show.IsDeleted && x.Show.EventID == EventID)
                    .AsNoTracking()
                    .Select(x => new TicketVM()
                    {
                        ID = x.ID,
                        ShowID = x.ShowID,
                        CostPrice = x.CostPrice,
                        FormattedSeatNumber = x.Seat != null && x.Seat.Column != null && x.Seat.Column.Row != null
                            ? (!x.IsReserved ? x.Seat.Column.Row.RowLabel + x.Seat.SeatNumber : x.Seat.Column.Row.RowLabel + x.Seat.SeatNumber + " (Reserved)")
                            : (!x.IsReserved ? $"Ticket #{x.ID}" : $"Ticket #{x.ID} (Reserved)"),
                    })
                    .ToList();
                return _ticketList ?? new List<TicketVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetTicketsByEventID in {_Title} Service due to {ex}");
            }
            return new List<TicketVM>();
        }
    }
}
