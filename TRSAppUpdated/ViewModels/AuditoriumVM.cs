using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.ViewModels
{
    public class AuditoriumVM : CommonModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(3, ErrorMessage = "Please fill out minimum 3 characters")]
        [StringLength(50, ErrorMessage = "Please fill out maximum 50 characters")]
        public string Title { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImagePath { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int NoOfRows { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public string StartingRow { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int CountryID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int CityID { get; set; }
        [Range(1,int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int VenueID { get; set; }
        public string? VenueTitle { get; set; }
        public string? VenueImagePath { get; set; }
       
    }
}
