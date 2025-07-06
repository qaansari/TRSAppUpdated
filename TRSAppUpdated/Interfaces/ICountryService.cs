using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface ICountryService
    {
        List<CountryVM> GetAll();
    }
}
