using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class ReservationTicket : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Ticket")]
        public int TicketID { get; set; }
        [ForeignKey("Reservation")]
        public int ReservationID { get; set; }
        [InverseProperty("Tickets")]
        public Ticket Ticket { get; set; }
        [InverseProperty("Reservations")]
        public Reservation Reservation { get; set; }
        [InverseProperty("Ticket")]
        public ICollection<Ticket> Tickets { get; set; }
        [InverseProperty("Reservation")]
        public ICollection<Reservation> Reservations { get; set; }
    }
}
