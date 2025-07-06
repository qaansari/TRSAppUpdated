using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class EventCategory
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Categories")]
        public int CategoryID { get; set; }
        [ForeignKey("Events")]
        public int EventID { get; set; }
        [ForeignKey("CreatedByUser")]
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public User? CreatedByUser { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Event> Events { get; set; }
    }
}
