using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class ReservationImage : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Reservations")]
        public int ReservationID { get; set; }
        public string ImagePath { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
