using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IEventStatusService
    {
        List<EventStatusVM> GetAll();
    }
}
