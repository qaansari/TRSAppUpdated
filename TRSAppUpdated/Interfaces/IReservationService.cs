using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IReservationService
    {
        ReservationDetailVM GetDetailsByID(int ID);
        Task<ReservationVM?> GetForEditAsync(int id, int eventId);
        Task<(bool Success, string Message, int EventId)> SaveReservationAsync(ReservationVM model, int currentUserId, bool isSuperAdmin);
        Task<(bool Success, string Message)> CloseReservationAsync(int id, int eventId, int currentUserId);
    }
}
