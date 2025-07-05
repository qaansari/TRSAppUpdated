namespace TRSAppUpdated.ViewModels
{
    public class RowSeatInputVM
    {
        public int RowID { get; set; }
        public string RowLabel { get; set; }
        public List<ColumnSeatInputVM> Columns { get; set; }
    }
    public class ColumnSeatInputVM
    {
        public int ColumnID { get; set; }
        public int ColumnNumber { get; set; }
    }
}
