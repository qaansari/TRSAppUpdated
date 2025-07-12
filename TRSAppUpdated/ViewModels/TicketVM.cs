using System.ComponentModel.DataAnnotations;

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
        [Range(1,double.MaxValue,ErrorMessage= "Please fill out this field")]
        public double Price { get; set; }
        public bool IsReserved { get; set; } = false;
    }
}
