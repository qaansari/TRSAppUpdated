using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey("City")]
        public int CityID { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string? ImagePath { get; set; }
        public City City { get; set; }
    }
}
