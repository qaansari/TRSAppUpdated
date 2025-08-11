using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IShowService
    {
        List<ShowVM> GetAll();
        List<ShowVM> GetActiveShowsByEventID(int EventID);
        List<ShowVM> GetAllShowsByEventID(int EventID);
    }
}
