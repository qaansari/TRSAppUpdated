using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    public class ShowController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ShowController> _logger;
        private readonly string _Title = "Show";
        private readonly int _UserID;
        public ShowController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            ILogger<ShowController> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
        }
        public IActionResult AddOrEdit(int ID = 0, int EventID = 0)
        {
            var _item = new ShowVM();

            if (ID != 0)
            {
                _item = _context.Shows.Where(x => !x.IsDeleted && x.ID == ID)
                       .Select(x => new ShowVM()
                       {
                           ID = x.ID,
                           EventID = x.EventID,
                           Date = x.Date,
                           Time = x.Time,
                           CreatedByUserID = x.CreatedByUserID,
                           CreatedDateTime = x.CreatedDateTime,
                       })
                       .AsNoTracking().FirstOrDefault();
            }
            if (EventID != 0)
            {
                _item.EventID = EventID;
            }
            return View(_item);
        }
        //AddOrEdit (POST)
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(ShowVM Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _item = new Show();
                    var _event = _context.Events.Where(x => !x.IsDeleted && x.ID == Model.ID).FirstOrDefault();
                    if (_event != null)
                    {
                        var _isValidDate = Model.Date >= _event.StartDate
                        && (_event.EndDate == null || _event.EndDate.HasValue && Model.Date <= _event.EndDate.Value);
                        if (!_isValidDate)
                        {
                            TempData["Message"] = "0,Invalid date selected";
                            return View(Model);
                        }
                        else
                        {
                            if (Model.ID != 0)
                            {
                                var isExists = _context.Shows.Where(x => !x.IsDeleted
                                    && x.ID != Model.ID
                                    && x.EventID == Model.EventID
                                    && x.Date == Model.Date
                                    && x.Time == Model.Time)
                                .Any();
                                if (isExists)
                                {
                                    TempData["Message"] = $"0,{_Title} Already Exists";
                                    return View(Model);
                                }

                                _item = _context.Shows.Where(x => !x.IsDeleted && x.ID == Model.ID)
                                    .AsNoTracking().FirstOrDefault();
                                if (_item != null)
                                {
                                    _item.Date = Model.Date.Value;
                                    _item.Time = Model.Time.Value;
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
                                var isExists = _context.Shows.Where(x => !x.IsDeleted
                                    && x.EventID == Model.EventID
                                    && x.Date == Model.Date
                                    && x.Time == Model.Time)
                                .Any();
                                if (isExists)
                                {
                                    TempData["Message"] = $"0,{_Title} Already Exists";
                                    return View(Model);
                                }
                                _item.EventID = Model.EventID;
                                _item.Date = Model.Date.Value;
                                _item.Time = Model.Time.Value;
                                _item.CreatedByUserID = _UserID;
                                _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                                _context.Shows.Add(_item);
                                await _context.SaveChangesAsync();
                                var isTicketExists = _context.Tickets.Where(x => !x.IsDeleted && x.ShowID == _item.ID).Any();
                                if (!isTicketExists)
                                {
                                    var _seatIDs =await _context.Seats.Include(x => x.Column)
                                     .ThenInclude(x => x.Row)
                                     .ThenInclude(x => x.Auditorium)
                                     .Where(x => x.Column.Row.AuditoriumID == _event.AuditoriumID)
                                     .Select(x => x.ID)
                                     .ToListAsync();
                                    var _tickets = _seatIDs.Select(seatId => new Ticket
                                    {
                                        ShowID = _item.ID,
                                        SeatID = seatId,
                                        CostPrice = 0,
                                        SalePrice = 0,
                                        Discount = 0,
                                        CreatedByUserID = _UserID,
                                        CreatedDateTime = DateTime.UtcNow.AddHours(5),
                                    }).ToList();
                                    await _context.Tickets.AddRangeAsync(_tickets);
                                    await _context.SaveChangesAsync();
                                }
                                TempData["Message"] = $"1,{_Title} Successfully Added";
                            }
                        }
                    }
                    return RedirectToAction("Details", "Event", new { ID = Model.EventID });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at AddOrEdit (POST) in {_Title} Controller due to {ex}");
            }
            return View(Model);
        }
    }
}
