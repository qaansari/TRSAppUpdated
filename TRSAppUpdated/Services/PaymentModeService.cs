using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class PaymentModeService : IPaymentModeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentModeService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Payment Mode";

        public PaymentModeService(AppDbContext context, ILogger<PaymentModeService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<PaymentModeVM> GetAll()
        {
            return _cacheService.GetOrCreate(CacheKeys.PaymentModes, () =>
            {
                try
                {
                    var result = _context.PaymentModes.Where(x => !x.IsDeleted)
                                 .Select(x => new PaymentModeVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<PaymentModeVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<PaymentModeVM>();
            }, TimeSpan.FromHours(4));
        }
    }
}
