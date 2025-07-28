using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface ITicketService
    {
        List<TicketVM> GetTicketsByEventID(int EventID);
        List<TicketForUsersVM> GetTicketsForUsersByEventID(int EventID);
    }
}
