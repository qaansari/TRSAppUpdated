using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IEventService
    {
        List<EventVM> GetAll();
        Task<(List<EventVM> _result, int totalRecords)> GetAllPaginated(int _pageNo = 1, int _pageSize = 10, string _searchedTerm = "", int _CountryID = 0);
        Task<EventVM> GetDetailsByID(int ID);
    }
}
