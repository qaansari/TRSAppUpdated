using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class Ticket : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Show")]
        public int ShowID { get; set; }
        [ForeignKey("Seat")]
        public int SeatID { get; set; }
        public double CostPrice { get; set; }
        public double Discount { get; set; }
        public double SalePrice { get; set; }
        public bool IsReserved { get; set; }
        public bool IsRefunded { get; set; }
        public bool IsCancelled { get; set; }
        [Column(TypeName="varchar(50)")]
        public string Category { get; set; } = string.Empty;
        public DateTime? ReservationDateTime { get; set; }
        public DateTime? RefundDateTime { get; set; }
        public DateTime? CancellationDateTime { get; set; }
        public Show Show { get; set; }
        public Seat Seat { get; set; }
        public ICollection<ReservationTicket> ReservationTickets { get; set; }
    }
}
