using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class NoteTypeService : INoteTypeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NoteTypeService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Note Type";

        public NoteTypeService(AppDbContext context, ILogger<NoteTypeService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<NoteTypeVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.NoteTypes, () =>
            {
                try
                {
                    var result = _context.NoteTypes.Where(x => !x.IsDeleted)
                                 .Select(x => new NoteTypeVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<NoteTypeVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<NoteTypeVM>();
            }, TimeSpan.FromHours(4));
        }
    }
}
