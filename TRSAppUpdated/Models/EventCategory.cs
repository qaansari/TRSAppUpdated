using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class EventCategory
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        [ForeignKey("Event")]
        public int EventID { get; set; }
        [ForeignKey("CreatedByUser")]
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public User? CreatedByUser { get; set; }
        public Category Category { get; set; }
        public Event Event { get; set; }
    }
}
