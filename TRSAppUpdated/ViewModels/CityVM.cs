using System.ComponentModel.DataAnnotations;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.ViewModels
{
    public class CityVM : CommonModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(3, ErrorMessage = "Please fill out minimum 3 characters")]
        [StringLength(40, ErrorMessage = "Please fill out maximum 40 characters")]
        public string Title { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int CountryID { get; set; }
        public string? CountryTitle { get; set; }
    }
}
