using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.ViewModels
{
    public class ReservationVM : CommonModel
    {
        public int ID { get; set; }
        public int EventID { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [StringLength(20, ErrorMessage = "Please fill out maximum 20 characters")]
        public string TicketNumber { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(4, ErrorMessage = "Please fill out minimum 4 characters")]
        [StringLength(41, ErrorMessage = "Please fill out maximum 41 characters")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(11, ErrorMessage = "Please fill out minimum 11 characters")]
        [StringLength(20, ErrorMessage = "Please fill out maximum 20 characters")]
        public string CustomerPhone { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public List<int>? TicketIDs { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Please fill out this field")]
        public double CostPrice { get; set; }
        public double Discount { get; set; }
        public double SalePrice { get; set; }
        public double AmountPaid { get; set; }
        public double AmountRemaining { get; set; }
        public bool IsFullyPaid { get; set; } = false;
        public IFormFile? ImageFile { get; set; }
        [MinLength(4, ErrorMessage = "Please fill out minimum 4 characters")]
        [StringLength(50, ErrorMessage = "Please fill out maximum 50 characters")]
        public string? TransactionID { get; set; }
        public DateTime? BookingDateTime { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int ShowID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int PaymentModeID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int ReservationStatusID { get; set; }
        public int? ClosedByUserID { get; set; }
        public DateTime? ClosingDateTime { get; set; }
        public bool? IsApproved { get; set; }
        public int? ActionByUserID { get; set; }
        public DateTime? ActionDateTime { get; set; }
        public string? ApprovedByUserName { get; set; }
        public string? PaymentModeTitle { get; set; }
        public string? ReservationStatusTitle { get; set; }
        public string? ShowDateTime { get; set; }
    }
    public class ReservationDetailVM : CommonModel
    {
        public int ID { get; set; }
        public string EventName { get; set; }
        public string TicketNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public bool IsFullyPaid { get; set; }
        public string SeatLabels  { get; set; }
        public string ReservationImages  { get; set; }=string.Empty;
        public double CostPrice { get; set; }
        public double Discount { get; set; }
        public double SalePrice { get; set; }
        public double AmountPaid { get; set; }
        public double? AmountRemaining { get; set; }
        public string? TransactionID { get; set; }
        public DateTime? BookingDateTime { get; set; }
        public string? PaymentModeTitle { get; set; }
        public string? ReservationStatusTitle { get; set; }
        public string? ShowDateTime { get; set; }
    }
}
