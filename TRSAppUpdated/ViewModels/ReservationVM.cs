using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.ViewModels
{
    public class ReservationVM : CommonModel
    {
        public int ID { get; set; }
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
        [Range(1, double.MaxValue, ErrorMessage = "Please fill out this field")]
        public double Discount { get; set; }
        public double SalePrice { get; set; }
        public double AmountPaid { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Please fill out this field")]
        public double AmountRemaining { get; set; }
        public bool IsFullyPaid { get; set; } = false;
        public List<IFormFile> ImageFiles { get; set; }
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
    }
}
