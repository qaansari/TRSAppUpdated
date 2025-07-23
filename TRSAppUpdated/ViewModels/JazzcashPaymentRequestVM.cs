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
        public string pp_TxnDateTime { get; set; } = "PKR";
    }
}
