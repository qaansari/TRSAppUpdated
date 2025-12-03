namespace TRSAppUpdated.ViewModels
{
    public class SeatVM
    {
        public string? EventName { get; set; }
        public DateTime? ShowDate { get; set; }
        public string? AuditoriumName { get; set; }
        public string? AuditoriumAlignment { get; set; }
        public string? VenueName { get; set; }
        public int ReservedSeats { get; set; }
        public double EstimatedRevenue { get; set; }
        public double AmountReceived { get; set; }
        public List<RowVM>? Rows { get; set; }
    }
}
