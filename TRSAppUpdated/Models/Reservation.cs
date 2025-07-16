using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class Reservation : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string TicketNumber { get; set; }
        [Column(TypeName = "varchar(41)")]
        public string CustomerName { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string CustomerPhone { get; set; }
        public double CostPrice { get; set; }
        public double Discount { get; set; }
        public double SalePrice { get; set; }
        public double AmountPaid { get; set; }
        public double AmountRemaining { get; set; }
        public bool IsFullyPaid { get; set; }
        [Column(TypeName="varchar(50)")]
        public string? TransactionID { get; set; }
        public DateTime? BookingDateTime { get; set; }
        [ForeignKey("Show")]
        public int ShowID { get; set; }
        [ForeignKey("PaymentMode")]
        public int PaymentModeID { get; set; }
        [ForeignKey("ReservationStatus")]
        public int ReservationStatusID { get; set; }
        [ForeignKey("ClosedByUser")]
        public int? ClosedByUserID { get; set; }
        public DateTime? ClosingDateTime { get; set; }
        public bool? IsApproved { get; set; }
        [ForeignKey("ActionByUser")]
        public int? ActionByUserID { get; set; }
        public DateTime? ActionDateTime { get; set; }
        public Show Show { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public ReservationStatus ReservationStatus { get; set; }
        public User? ClosedByUser { get; set; }
        public User? ActionByUser { get; set; }
        public ICollection<ReservationTicket> ReservationTickets { get; set; }
        public ICollection<ReservationImage> ReservationImages { get; set; }
    }
}
