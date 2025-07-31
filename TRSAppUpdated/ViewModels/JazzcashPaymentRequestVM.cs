using System.ComponentModel.DataAnnotations;

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
        public int ShowID { get; set; }
        public int EventID { get; set; }
        public double Amount { get; set; }
        public string TicketNumber { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [StringLength(41)]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [StringLength(20)]
        public string CustomerPhone { get; set; }
        public List<TicketForUsersVM>? TicketList { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
