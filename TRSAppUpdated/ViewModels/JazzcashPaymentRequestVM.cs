namespace TRSAppUpdated.ViewModels
{
    public class JazzcashPaymentRequestVM
    {
        public string pp_Version { get; set; } = "1.1";
        public string pp_TxnType { get; set; } = "MWALLET";
        public string pp_Language { get; set; } = "EN";
        public string pp_MerchantID { get; set; }
        public string pp_Password { get; set; }
        public string pp_TxnRefNo { get; set; }
        public string pp_Amount { get; set; }
        public string pp_TxnCurrency { get; set; } = "PKR";
        public string pp_TxnDateTime { get; set; }
        public string pp_BillReference { get; set; }
        public string pp_Description { get; set; }
        public string pp_ReturnURL { get; set; }
        public string pp_SecureHash { get; set; }
    }
    public class PaymentMethodRequestVM
    {
        public double Amount { get; set; }
        public string TicketNo { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
