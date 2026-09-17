using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExpenseService> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Expense";

        public ExpenseService(AppDbContext context, ILogger<ExpenseService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        //Get All with In-Memory Caching
        public List<ExpenseVM> GetAll()
        {
            return _cacheService.GetOrCreate("LOOKUP_EXPENSES", () =>
            {
                try
                {
                    var result = _context.Expenses.Where(x => !x.IsDeleted)
                                 .Select(x => new ExpenseVM()
                                 {
                                     ID = x.ID,
                                     Title = x.Title,
                                 })
                                 .AsNoTracking()
                                 .OrderBy(x => x.Title)
                                 .ToList();
                    return result ?? new List<ExpenseVM>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
                }
                return new List<ExpenseVM>();
            }, TimeSpan.FromHours(4));
        }
    }
}
