namespace TRSAppUpdated.ViewModels
{
    public class PaymentMethodRequestVM
    {
        public double Amount { get; set; }
        public int ReservationID { get; set; }
        public string Description { get; set; }=string.Empty;
    }
}
