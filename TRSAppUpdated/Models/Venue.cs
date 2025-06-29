using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class Venue : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Title { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string Address { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string? ImagePath { get; set; }
        public ICollection<Auditorium> Auditoriums { get; set; }
    }
}
