using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class ReservationTicket
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Ticket")]
        public int TicketID { get; set; }
        [ForeignKey("Reservation")]
        public int ReservationID { get; set; }
        [ForeignKey("CreatedByUser")]
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public User? CreatedByUser { get; set; }
        public Ticket Ticket { get; set; }
        public Reservation Reservation { get; set; }
    }
}
