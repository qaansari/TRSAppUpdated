using System.ComponentModel.DataAnnotations;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.ViewModels
{
    public class CountryVM : CommonModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(3, ErrorMessage = "Please fill out minimum 3 characters")]
        [StringLength(40, ErrorMessage = "Please fill out maximum 40 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(2, ErrorMessage = "Please fill out minimum 2 characters")]
        [StringLength(4, ErrorMessage = "Please fill out maximum 4 characters")]
        public string DialingCode { get; set; }
        public string Flag { get; set; }
    }
}
