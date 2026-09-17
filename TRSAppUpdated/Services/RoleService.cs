using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RoleService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Role";

        public RoleService(AppDbContext context, ILogger<RoleService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<RoleVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.Roles, () =>
            {
                try
                {
                    var result = _context.Roles.Where(x => !x.IsDeleted && x.ID != 1)
                                 .Select(x => new RoleVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<RoleVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<RoleVM>();
            }, TimeSpan.FromHours(4));
        }
    }
}
