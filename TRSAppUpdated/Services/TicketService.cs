using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Inputs;
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
                var showIDList = _context.Shows.Where(x => !x.IsDeleted && !x.IsEnded && x.EventID == EventID)
                                                   .AsNoTracking()
                                                   .Select(x => x.ID)
                                                   .ToList();
                var _ticketList = _context.Tickets
                    .Where(x => showIDList.Contains(x.ShowID) && !x.IsDeleted)
                    .AsNoTracking()
                    .Select(x => new TicketVM()
                    {
                        ID = x.ID,
                        ShowID = x.ShowID,
                        CostPrice = x.CostPrice,
                        FormattedSeatNumber = !x.IsReserved ? x.Seat.Column.Row.RowLabel + x.Seat.SeatNumber : x.Seat.Column.Row.RowLabel + x.Seat.SeatNumber + " " + "(Reserved)",
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
        public List<TicketForUsersVM> GetTicketsForUsersByEventID(int EventID)
        {
            var showIDList = _context.Shows.Where(x => !x.IsDeleted && !x.IsEnded && x.EventID == EventID)
                                                  .AsNoTracking()
                                                  .Select(x => x.ID)
                                                  .ToList();
            var _ticketList = _context.Tickets
                .Where(x => showIDList.Contains(x.ShowID) && !x.IsDeleted && !x.IsReserved)
                .AsNoTracking()
                .GroupBy(x => x.CostPrice)
                .Select(x => new TicketForUsersVM()
                {
                    ID = x.FirstOrDefault().ID,
                    ShowID = x.FirstOrDefault().ShowID,
                    CostPrice = (x.Key) * 20 / 100 + x.Key,
                    SalePrice = x.FirstOrDefault().SalePrice,
                    AvailableSeats = x.Count()
                })
                .ToList();
            return _ticketList ?? new List<TicketForUsersVM>();
        }
    }
}
