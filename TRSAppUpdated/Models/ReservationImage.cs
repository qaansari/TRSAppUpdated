using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class ReservationImage : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Reservation")]
        public int ReservationID { get; set; }
        public string ImagePath { get; set; }
        [InverseProperty("Reservations")]
        public Reservation Reservation { get; set; }
        [InverseProperty("Reservation")]
        public ICollection<Reservation> Reservations { get; set; }
    }
}
