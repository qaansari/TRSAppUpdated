using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class Auditorium : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Title { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string? ImagePath { get; set; }
        [ForeignKey("Venue")]
        public int VenueID { get; set; }
        public Venue Venue { get; set; }
        public ICollection<Row> Rows { get; set; }
    }
}
