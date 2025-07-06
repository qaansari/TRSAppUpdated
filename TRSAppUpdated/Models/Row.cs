using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class Row
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(1)")]
        public string RowLabel { get; set; }
        [ForeignKey("Auditorium")]
        public int AuditoriumID { get; set; }
        public Auditorium Auditorium { get; set; }
        [ForeignKey("CreatedByUser")]
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public User? CreatedByUser { get; set; }
        public ICollection<Column> Columns { get; set; }
    }
}
