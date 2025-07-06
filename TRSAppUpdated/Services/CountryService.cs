using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class CountryService : ICountryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CountryService> _logger;
        private readonly string _Title = "Country";
        public CountryService(AppDbContext context, ILogger<CountryService> logger)
        {
            _context = context;
            _logger = logger;
        }
        //Get All
        public List<CountryVM> GetAll()
        {
            try
            {
                var _result = _context.Countries.Where(x => !x.IsDeleted)
                             .Select(x => new CountryVM()
                             {
                                 ID = x.ID,
                                 Title = x.Title,
                             })
                             .AsNoTracking()
                             .OrderBy(x => x.Title)
                             .ToList();
                return _result ?? new List<CountryVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
            }
            return new List<CountryVM>();
        }
    }
}
