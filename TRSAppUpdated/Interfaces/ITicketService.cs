using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface ITicketService
    {
        List<TicketVM> GetTicketsByEventID(int EventID);
        TicketBookingVM GetTicketsForUsersByEventID(int EventID);
    }
}
