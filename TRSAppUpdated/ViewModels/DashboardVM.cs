namespace TRSAppUpdated.ViewModels
{
    public class DashboardVM
    {
        public int TotalUsers { get; set; } = 0;
        public int TotalVenues { get; set; } = 0;
        public int TotalEvents { get; set; } = 0;
        public string EventTitle { get; set; } = string.Empty;
        public string? EventImagePath { get; set; } = string.Empty;
        public double? Expense { get; set; } = 0;
        public double? Revenue { get; set; } = 0;
        public double? Profit { get; set; } = 0;
    }
}
