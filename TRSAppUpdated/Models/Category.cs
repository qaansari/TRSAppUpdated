using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class Category : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string Title { get; set; }
        [InverseProperty("Categories")]
        public ICollection<EventCategory> EventCategoriesList { get; set; }
        [InverseProperty("Category")]
        public ICollection<EventCategory> EventCategories { get; set; }
    }
}
