using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IReservationStatusService
    {
        List<ReservationStatusVM> GetAll();
    }
}
