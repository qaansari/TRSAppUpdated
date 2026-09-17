using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class ShowService : IShowService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ShowService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Show";

        public ShowService(AppDbContext context, ILogger<ShowService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        public List<ShowVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.ShowsList, () =>
            {
                try
                {
                    var result = _context.Shows.Where(x => !x.IsDeleted)
                                 .Select(x => new ShowVM()
                                 {
                                     ID = x.ID,
                                     Date = x.Date,
                                     Time = x.Time,
                                     EventID = x.EventID,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.EventID)
                                 .ToList();
                    return result ?? new List<ShowVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<ShowVM>();
            }, TimeSpan.FromHours(1));
        }

        //Get Active Shows By EventID
        public List<ShowVM> GetActiveShowsByEventID(int EventID)
        {
            try
            {
                var list = _context.Shows
                        .Where(x => !x.IsDeleted && !x.IsEnded && x.EventID == EventID)
                        .Select(x => new ShowVM()
                        {
                            ID = x.ID,
                            Date = x.Date,
                            Time = x.Time
                        })
                        .AsNoTracking().ToList();
                return list ?? new List<ShowVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetActiveShowsByEventID in {_Title} Service due to {ex}");
            }
            return new List<ShowVM>();
        }

        //Get All Shows By EventID
        public List<ShowVM> GetAllShowsByEventID(int EventID)
        {
            try
            {
                var list = _context.Shows
                        .Where(x => !x.IsDeleted && x.EventID == EventID)
                        .Select(x => new ShowVM()
                        {
                            ID = x.ID,
                            Date = x.Date,
                            Time = x.Time
                        })
                        .AsNoTracking().ToList();
                return list ?? new List<ShowVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetAllShowsByEventID in {_Title} Service due to {ex}");
            }
            return new List<ShowVM>();
        }
    }
}
