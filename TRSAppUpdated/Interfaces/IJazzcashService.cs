using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IJazzcashService
    {
        JazzcashPaymentRequestVM CreatePaymentRequest(PaymentMethodRequestVM Model);
        
    }
}
