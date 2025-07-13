using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.ViewModels
{
    public class ShowExpenseVM : CommonModel
    {
        public int ID { get; set; }
        [Range(1,int.MaxValue,ErrorMessage ="Please fill out this field")]
        public int EventID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int ShowID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int ExpenseID { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public DateTime? Date { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public double Amount { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public string Description { get; set; }
    }
}
