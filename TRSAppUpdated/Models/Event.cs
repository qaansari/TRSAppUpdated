using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class Event : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public bool IsGeneral { get; set; }
        public bool IsEnded { get; set; }
        public string? ImagePath { get; set; }
        [ForeignKey("Auditorium")]
        public int AuditoriumID { get; set; }
        public Auditorium Auditorium { get; set; }
        public ICollection<EventCategory> EventCategories { get; set; }
        public ICollection<Show> Shows { get; set; }
        public ICollection<ShowExpense> ShowExpenses { get; set; }
    }
}
