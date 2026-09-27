using System.ComponentModel.DataAnnotations;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.ViewModels
{
    public class ReservationVM : CommonModel
    {
        public int ID { get; set; }
        public int EventID { get; set; }

        [Required(ErrorMessage = "Please enter ticket number")]
        [StringLength(20, ErrorMessage = "Ticket number cannot exceed 20 characters")]
        public string TicketNumber { get; set; }

        [Required(ErrorMessage = "Please enter customer name")]
        [MinLength(3, ErrorMessage = "Customer name must be at least 3 characters")]
        [StringLength(50, ErrorMessage = "Customer name cannot exceed 50 characters")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Please enter customer phone")]
        [MinLength(10, ErrorMessage = "Customer phone must be at least 10 characters")]
        [StringLength(20, ErrorMessage = "Customer phone cannot exceed 20 characters")]
        public string CustomerPhone { get; set; }

        [Required(ErrorMessage = "Please select at least one ticket")]
        public List<int>? TicketIDs { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid cost price")]
        public double CostPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid discount")]
        public double Discount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid sale price")]
        public double SalePrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid amount paid")]
        public double AmountPaid { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid amount remaining")]
        public double AmountRemaining { get; set; }

        public bool IsFullyPaid { get; set; } = false;
        public IFormFile? ImageFile { get; set; }

        [StringLength(50, ErrorMessage = "Transaction ID cannot exceed 50 characters")]
        public string? TransactionID { get; set; }

        public DateTime? BookingDateTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a show")]
        public int ShowID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a payment mode")]
        public int PaymentModeID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a reservation status")]
        public int ReservationStatusID { get; set; }

        public int? ClosedByUserID { get; set; }
        public DateTime? ClosingDateTime { get; set; }
        public bool? IsApproved { get; set; }
        public int? ActionByUserID { get; set; }
        public DateTime? ActionDateTime { get; set; }
        public string? ApprovedByUserName { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? PaymentModeTitle { get; set; }
        public string? ReservationStatusTitle { get; set; }
        public string? ShowDateTime { get; set; }
    }
    public class ReservationDetailVM : CommonModel
    {
        public int ID { get; set; }
        public string EventName { get; set; }
        public string EventImagePath { get; set; } = string.Empty;
        public string TicketNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public bool IsFullyPaid { get; set; }
        public string SeatLabels { get; set; }
        public string ReservationImages { get; set; } = string.Empty;
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
        public string? ClosedByUserName { get; set; }
        public string? ClosedDateTime{ get; set; }
        public string? TicketCategory { get; set; }
    }
}
