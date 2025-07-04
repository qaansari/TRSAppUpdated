using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.ViewModels
{
    public class RowWithColumnVM
    {
        public int RowID { get; set; }
        public string RowLabel { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please fill out this field")]
        public int ColumnCount { get; set; }
    }
}
