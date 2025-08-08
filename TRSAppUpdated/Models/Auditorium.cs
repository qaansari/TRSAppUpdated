using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int NoOfRows { get; set; }
        [Column(TypeName = "varchar(1)")]
        public string StartingRow { get; set; }
        [Column(TypeName = "varchar(5)")]
        public string Alignment { get; set; }
        [ForeignKey("Venue")]
        public int VenueID { get; set; }
        public Venue Venue { get; set; }
        public ICollection<Row> Rows { get; set; }
        public ICollection<Event> Events { get; set; }
    }
}
