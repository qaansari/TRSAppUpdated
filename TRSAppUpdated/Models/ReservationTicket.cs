using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class ReservationTicket : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Tickets")]
        public int TicketID { get; set; }
        [ForeignKey("Reservations")]
        public int ReservationID { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
