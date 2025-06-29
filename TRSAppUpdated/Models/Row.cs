using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class Row : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(1)")]
        public string RowLabel { get; set; }
        [ForeignKey("Auditorium")]
        public int AuditoriumID { get; set; }
        public Auditorium Auditorium { get; set; }
        public ICollection<Column> Columns { get; set; }
    }
}
