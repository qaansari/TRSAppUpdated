using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    public class ShowExpenseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ShowExpenseController> _logger;
        private readonly IExpenseService _expenseService;
        private readonly IShowService _showService;
        private readonly string _Title = "Show Expense";
        private readonly int _UserID;
        public ShowExpenseController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            ILogger<ShowExpenseController> logger,
            IExpenseService expenseService, IShowService showService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _expenseService = expenseService;
            _showService = showService;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
        }
        //Index (Get All)
        public async Task<IActionResult> Index(int EventID, int _pageNo = 1, int _pageSize = 10, string _searchedTerm = "")
        {
            try
            {
                if (EventID != 0)
                {
                    ViewBag.EventID = EventID;
                }
                var (_result, totalRecords) = await GetList(EventID, _pageNo, _pageSize, _searchedTerm);
                ViewBag.TotalRecords = totalRecords;
                ViewBag.PageNo = _pageNo;
                ViewBag.PageSize = _pageSize;
                ViewBag.SearchedTerm = _searchedTerm;
                return View(_result ?? new List<ShowExpenseVM>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index in {_Title} Controller due to {ex}");
            }
            return View(new List<ShowExpenseVM>());
        }
        // GetPartial
        [HttpGet]
        public async Task<IActionResult> GetPartial(int EventID, int _pageNo = 1, int _pageSize = 10, string _searchedTerm = "")
        {
            var _partialViewName = _Title.Contains(" ") ? _Title.Replace(" ", "") : _Title;
            try
            {
                var (_result, totalRecords) = await GetList(EventID, _pageNo, _pageSize, _searchedTerm);
                ViewBag.TotalRecords = totalRecords;
                ViewBag.PageNo = _pageNo;
                ViewBag.PageSize = _pageSize;
                ViewBag.SearchedTerm = _searchedTerm;
                return PartialView($"_{_partialViewName}TablePartial", _result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetPartial in {_Title} Controller due to {ex}");
            }
            return PartialView($"_{_partialViewName}TablePartial", new List<ShowExpenseVM>());
        }
        //AddOrEdit (GET)
        public IActionResult AddOrEdit(int ID = 0, int EventID = 0)
        {
            var _item = new ShowExpenseVM();
            try
            {
                if (EventID != 0)
                {
                    ViewBag.EventID = EventID;
                }
                ViewData["ExpenseList"] = _expenseService.GetAll();
                ViewData["ShowList"] = _showService.GetAllShowsByEventID(EventID);
                if (ID != 0)
                {
                    _item = _context.ShowExpenses.Where(x => !x.IsDeleted && x.ID == ID)
                        .Select(x => new ShowExpenseVM()
                        {
                            ID = x.ID,
                            ShowID = x.ShowID,
                            EventID = x.EventID,
                            ExpenseID = x.ExpenseID,
                            Date = x.Date,
                            Amount = x.Amount,
                            Description = x.Description,
                            CreatedByUserID = x.CreatedByUserID,
                            CreatedDateTime = x.CreatedDateTime,
                        })
                        .AsNoTracking().FirstOrDefault();
                }
                else
                {
                    _item.EventID = EventID;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at AddOrEdit (GET) in {_Title} Controller due to {ex}");
            }
            return View(_item);
        }
        //AddOrEdit (POST)
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(ShowExpenseVM Model)
        {
            try
            {
                if (Model.EventID != 0)
                {
                    ViewBag.EventID = Model.EventID;
                }
                ViewData["ExpenseList"] = _expenseService.GetAll();
                ViewData["ShowList"] = _showService.GetAllShowsByEventID(Model.EventID);
                if (ModelState.IsValid)
                {
                    var _item = new ShowExpense();
                    if (Model.ID != 0)
                    {
                        _item = _context.ShowExpenses.Where(x => !x.IsDeleted && x.ID == Model.ID)
                            .AsNoTracking().FirstOrDefault();
                        if (_item != null)
                        {
                            _item.ShowID = Model.ShowID;
                            _item.ExpenseID = Model.ExpenseID;
                            _item.Date = Model.Date.Value;
                            _item.Amount = Model.Amount;
                            _item.Description = Model.Description;
                            _item.UpdatedByUserID = _UserID;
                            _item.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                            _context.ChangeTracker.Clear();
                            _context.Entry(_item).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                            TempData["Message"] = $"1,{_Title} Successfully Updated";
                        }
                    }
                    else
                    {
                        _item.EventID = Model.EventID;
                        _item.ShowID = Model.ShowID;
                        _item.ExpenseID = Model.ExpenseID;
                        _item.Date = Model.Date.Value;
                        _item.Amount = Model.Amount;
                        _item.Description = Model.Description;
                        _item.CreatedByUserID = _UserID;
                        _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.ShowExpenses.Add(_item);
                        await _context.SaveChangesAsync();
                        TempData["Message"] = $"1,{_Title} Successfully Added";
                    }
                    return RedirectToAction("Index");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error occured at AddOrEdit (POST) in {_Title} Controller due to {ex}");
            }
            return View(Model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int ID, int EventID)
        {
            try
            {
                var _item = _context.ShowExpenses.Where(x => !x.IsDeleted && x.ID == ID)
                                  .AsNoTracking().FirstOrDefault();
                if (_item != null)
                {
                    _item.IsDeleted = true;
                    _item.DeletedByUserID = _UserID;
                    _item.DeletedDateTime = DateTime.UtcNow.AddHours(5);
                    _context.ChangeTracker.Clear();
                    _context.Entry(_item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"1,{_Title} Successfully Deleted";
                }
                else
                {
                    TempData["Message"] = $"0,No {_Title} Found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Delete (POST) in {_Title} Controller due to {ex}");
            }
            return RedirectToAction("Index", "ShowExpense", new { EventID = EventID });
        }
        //Get List
        [NonAction]
        public async Task<(List<ShowExpenseVM> _result, int totalRecords)> GetList(int EventID, int _pageNo, int _pageSize, string _searchedTerm)
        {

            var _result = new List<ShowExpenseVM>();
            int totalRecords = 0;
            try
            {
                // For AJAX calls
                var _query = _context.ShowExpenses.Where(x => !x.IsDeleted && x.EventID == EventID)
                           .Select(x => new ShowExpenseVM()
                           {
                               ID = x.ID,
                               Date = x.Date,
                               Amount = x.Amount,
                               Description = x.Description,
                               ShowDateTime = x.Show.Date.ToString("dddd, dd MMMM yyyy") + " " + DateTime.Today.Add(x.Show.Time).ToString("hh:mm tt"),
                               ExpenseTitle = x.Expense.Title
                           })
                           .AsNoTracking()
                           .AsQueryable();
                if (!string.IsNullOrEmpty(_searchedTerm))
                {
                    _query = _query
                        .Where(x => (!string.IsNullOrEmpty(x.ShowDateTime) && x.ShowDateTime.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower()))
                        || !string.IsNullOrEmpty(x.ExpenseTitle) && x.ExpenseTitle.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower()));
                }
                int _skip = (_pageNo - 1) * _pageSize;
                totalRecords = await _query.CountAsync();
                _query = _query.Skip(_skip).Take(_pageSize);
                _result = await _query.OrderByDescending(x => x.ID).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetList in {_Title} Controller due to {ex}");
            }
            return (_result, totalRecords);
        }
    }
}
