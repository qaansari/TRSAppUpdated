using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class Column
    {
        [Key]
        public int ID { get; set; }
        public int ColumnNumber { get; set; }
        [ForeignKey("Row")]
        public int RowID { get; set; }
        public Row Row { get; set; }
        [ForeignKey("CreatedByUser")]
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public User? CreatedByUser { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
