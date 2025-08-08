using System.ComponentModel.DataAnnotations;
using TRSAppUpdated.Models;

namespace TRSAppUpdated.ViewModels
{
    public class ShowVM : CommonModel
    {
        public int ID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int EventID { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public string TimeString { get; set; }
        public bool IsEnded { get; set; } = false;
        public string? ShowDateTime
        {
            get
            {
                if (this.Date.HasValue && this.Time.HasValue)
                {
                    return this.Date.Value.ToString("dddd, dd MMMM yyyy") + " " + DateTime.Today.Add(this.Time.Value).ToString("hh:mm tt");
                }
                return "-";
            }
        }
    }
}
