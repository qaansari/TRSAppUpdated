using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
