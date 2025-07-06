using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class Seat
    {
        [Key]
        public int ID { get; set; }
        public int SeatNumber { get; set; }
        [ForeignKey("Column")]
        public int ColumnID { get; set; }
        public Column Column { get; set; }
        [ForeignKey("CreatedByUser")]
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public User? CreatedByUser { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
