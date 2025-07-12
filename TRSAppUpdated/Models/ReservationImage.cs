using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class ReservationImage
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Reservation")]
        public int ReservationID { get; set; }
        public string ImagePath { get; set; }
        [ForeignKey("CreatedByUser")]
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public User? CreatedByUser { get; set; }
        public Category Category { get; set; }
        public Reservation Reservation { get; set; }
    }
}
