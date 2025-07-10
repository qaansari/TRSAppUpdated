using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IShowService
    {
        List<ShowVM> GetShowsByEventID(int EventID);
    }
}
