using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class Column : CommonModel
    {
        [Key]
        public int ID { get; set; }
        public int ColumnNumber { get; set; }
        [ForeignKey("Row")]
        public int RowID { get; set; }
        public Row Row { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
