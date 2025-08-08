using System.ComponentModel.DataAnnotations;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.ViewModels
{
    public class VenueVM : CommonModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(3, ErrorMessage = "Please fill out minimum 3 characters")]
        [StringLength(50, ErrorMessage = "Please fill out maximum 50 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(10, ErrorMessage = "Please fill out minimum 10 characters")]
        [StringLength(300, ErrorMessage = "Please fill out maximum 300 characters")]
        public string Address { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int CountryID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int CityID { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImagePath { get; set; }
        public string? CityTitle { get; set; }
        public string? CountryTitle { get; set; }
    }
}
