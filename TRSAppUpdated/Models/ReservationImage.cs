using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class ReservationImage
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Reservations")]
        public int ReservationID { get; set; }
        public string ImagePath { get; set; }
        [ForeignKey("CreatedByUser")]
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public User? CreatedByUser { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
