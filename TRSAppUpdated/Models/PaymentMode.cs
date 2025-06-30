using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class PaymentMode : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(40)")]
        public string Title { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
