using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IPaymentModeService
    {
        List<PaymentModeVM> GetAll();
    }
}
