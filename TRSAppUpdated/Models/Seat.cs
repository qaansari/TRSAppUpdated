using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class Seat : CommonModel
    {
        [Key]
        public int ID { get; set; }
        public int SeatNumber { get; set; }
        [ForeignKey("Column")]
        public int ColumnID { get; set; }
        public Column Column { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
