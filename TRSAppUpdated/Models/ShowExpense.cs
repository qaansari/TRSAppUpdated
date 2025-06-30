using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class ShowExpense : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Event")]
        public int EventID { get; set; }
        [ForeignKey("Show")]
        public int ShowID { get; set; }
        [ForeignKey("Expense")]
        public int ExpenseID { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public Expense Expense { get; set; }
        public Event Event { get; set; }
        public Show Show { get; set; }
    }
}
