using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IShowService
    {
        List<ShowVM> GeActiveShowsByEventID(int EventID);
    }
}
