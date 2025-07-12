using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IReservationService
    {
        ReservationDetailVM GetDetailsByID(int ID);
    }
}
