using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class PaymentModeService : IPaymentModeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentModeService> _logger;
        private readonly string _Title = "Payment Mode";
        public PaymentModeService(AppDbContext context, ILogger<PaymentModeService> logger)
        {
            _context = context;
            _logger = logger;
        }
        //Get All
        public List<PaymentModeVM> GetAll()
        {
            try
            {
                var _result = _context.PaymentModes.Where(x => !x.IsDeleted)
                             .Select(x => new PaymentModeVM()
                             {
                                 ID = x.ID,
                                 Title = x.Title,
                             })
                             .AsNoTracking()
                             .OrderBy(x => x.Title)
                             .ToList();
                return _result ?? new List<PaymentModeVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
            }
            return new List<PaymentModeVM>();
        }
    }
}
