using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateTicketPdf(ReservationDetailVM reservation, bool isGeneralEvent, string webRootPath);
    }
}

