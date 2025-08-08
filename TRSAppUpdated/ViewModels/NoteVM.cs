using System.ComponentModel.DataAnnotations;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.ViewModels
{
    public class NoteVM : CommonModel
    {
        public int ID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int Seq { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public string EnTitle { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public string UrduTitle { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int NoteTypeID { get; set; }
        public string? NoteTypeTitle { get; set; }
    }
}
