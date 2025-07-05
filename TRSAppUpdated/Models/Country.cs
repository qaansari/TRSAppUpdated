using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class Country : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(40)")]
        public string Title { get; set; }
        [Column(TypeName = "varchar(4)")]
        public string DialingCode { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string Flag { get; set; }
        public ICollection<City> Cities { get; set; }
    }
}
