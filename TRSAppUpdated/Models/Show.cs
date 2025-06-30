using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class Show : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Event")]
        public int EventID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public bool IsEnded { get; set; }
        public Event Event { get; set; }
        public ICollection<ShowExpense> ShowExpenses { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
