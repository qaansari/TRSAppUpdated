using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class Venue : CommonModel
    {
        [Key]
        [Column(TypeName = "varchar(70)")]
        public string ID { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Title { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string Address { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string? ImagePath { get; set; }
    }
}
