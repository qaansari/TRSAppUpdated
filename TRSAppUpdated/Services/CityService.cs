using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class CityService: ICityService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CityService> _logger;
        private readonly string _Title = "City";
        public CityService(AppDbContext context, ILogger<CityService> logger)
        {
            _context = context;
            _logger = logger;
        }
        //Get All
        public List<CityVM> GetAll()
        {
            try
            {
                var _result = _context.Cities.Where(x => !x.IsDeleted)
                             .Select(x => new CityVM()
                             {
                                 ID = x.ID,
                                 Title = x.Title,
                                 CountryID = x.CountryID,
                             })
                             .AsNoTracking()
                             .OrderBy(x => x.Title)
                             .ToList();
                return _result ?? new List<CityVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
            }
            return new List<CityVM>();
        }
    }
}
