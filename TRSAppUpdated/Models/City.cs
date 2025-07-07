using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class City : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(40)")]
        public string Title { get; set; }
        [ForeignKey("Country")]
        public int CountryID { get; set; }
        public Country Country { get; set; }
        public ICollection<Venue> Venues { get; set; }
    }
}
