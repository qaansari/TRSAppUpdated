using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace TRSAppUpdated.ViewModels
{
    public class TicketVM
    {
        public int ID { get; set; }
        public int ShowID { get; set; }
        public int SeatNumber { get; set; }
        public double CostPrice { get; set; }
        public string FormattedSeatNumber { get; set; }
        public bool IsReserved { get; set; }
    }
    public class TicketForUsersVM
    {
        public int ID { get; set; }
        public int ShowID { get; set; }
        public int AvailableSeats { get; set; }
        public double CostPrice { get; set; }
        public double SalePrice { get; set; }
        public int RequiredSeats { get; set; }
        public string Category { get; set; }
    }
    public class TicketBookingVM()
    {
        public int ShowID { get; set; }
        public List<TicketForUsersVM> TicketList { get; set; }
    }
    //Seat Ticket Form VM
    public class TicketsBySeatVM
    {
        public int EventID { get; set; }
        public int ShowID { get; set; }
        public List<TicketRowVM> TicketRowList { get; set; }
    }
    //Ticket Row
    public class TicketRowVM
    {
        public int ID { get; set; }
        public string RowLabel { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Please fill out this field")]
        public double Price { get; set; }
        public bool IsReserved { get; set; } = false;
        public bool IsRefunded { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        [Required(ErrorMessage = "Please fill out this field")]
        public string Category { get; set; }
    }
}
