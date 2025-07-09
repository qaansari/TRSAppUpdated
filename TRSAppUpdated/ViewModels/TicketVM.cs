namespace TRSAppUpdated.ViewModels
{
    public class TicketVM
    {

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
        public string Row { get; set; }
        public double Price { get; set; }
        public bool IsReserved { get; set; } = false;
    }
}
