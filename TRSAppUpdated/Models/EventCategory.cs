using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class EventCategory : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        [ForeignKey("Event")]
        public int EventID { get; set; }
        [InverseProperty("Categories")]
        public Category Category { get; set; }
        [InverseProperty("Events")]
        public Event Event { get; set; }
        [InverseProperty("Category")]
        public ICollection<Category> Categories { get; set; }
        [InverseProperty("Event")]
        public ICollection<Event> Events { get; set; }
    }
}
