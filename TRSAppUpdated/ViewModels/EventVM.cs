using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.ViewModels
{
    public class EventVM : CommonModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(4, ErrorMessage = "Please fill out minimum 3 characters")]
        [StringLength(50, ErrorMessage = "Please fill out maximum 50 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public bool IsGeneral { get; set; } = false;
        public string? ImagePath { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int CountryID { get; set; }
        [Range(1,int.MaxValue,ErrorMessage = "Please fill out this field")]
        public int CityID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int VenueID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int AuditoriumID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int EventStatusID { get; set; }
    }
}
