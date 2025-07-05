namespace TRSAppUpdated.ViewModels
{
    public class RowVM
    {
        public int ID { get; set; }
        public string RowLabel { get; set; }
        public List<ColumnVM> Columns { get; set; }
    }
}
