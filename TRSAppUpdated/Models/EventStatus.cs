using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class EventStatus : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(40)")]
        public string Title { get; set; }
        public ICollection<Event> Events { get; set; }
    }
}
