using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class ShowService : IShowService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ShowService> _logger;
        private readonly string _Title = "Show";
        public ShowService(AppDbContext context, ILogger<ShowService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<ShowVM> GetAll()
        {
            try
            {
                var _result = _context.Shows.Where(x => !x.IsDeleted)
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
                return _result ?? new List<ShowVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
            }
            return new List<ShowVM>();
        }
        //Get Active Shows By EventID
        public List<ShowVM> GetActiveShowsByEventID(int EventID)
        {
            try
            {
                var _list = _context.Shows
                        .Where(x => !x.IsDeleted && !x.IsEnded && x.EventID == EventID)
                        .Select(x => new ShowVM()
                        {
                            ID = x.ID,
                            Date = x.Date,
                            Time = x.Time
                        })
                        .AsNoTracking().ToList();
                return _list ?? new List<ShowVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetShowsByEventID in {_Title} Service due to {ex}");
            }
            return new List<ShowVM>();
        }
        //Get Active Shows By EventID
        public List<ShowVM> GetAllShowsByEventID(int EventID)
        {
            try
            {
                var _list = _context.Shows
                        .Where(x => !x.IsDeleted && x.EventID == EventID)
                        .Select(x => new ShowVM()
                        {
                            ID = x.ID,
                            Date = x.Date,
                            Time = x.Time
                        })
                        .AsNoTracking().ToList();
                return _list ?? new List<ShowVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetShowsByEventID in {_Title} Service due to {ex}");
            }
            return new List<ShowVM>();
        }
    }
}
