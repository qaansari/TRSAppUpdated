using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.ViewModels
{
    public class ColumnVM
    {
        public int ID { get; set; }
        public int ColumnNumber { get; set; }
        public int RowID { get; set; }
        public List<int>? Seats { get; set; }
        public List<TicketVM>? SeatTickets { get; set; }
    }
}
