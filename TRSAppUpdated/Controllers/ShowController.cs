using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Xml.Linq;
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
        [Authorize(Roles = "Super Admin,Admin")]
        public IActionResult AddOrEdit(int ID = 0, int EventID = 0)
        {
            var _item = new ReservationVM();
            try
            {
                if (EventID != 0)
                {
                    ViewBag.EventID = EventID;
                }
                if (ID != 0)
                {
                    _item = _context.Shows.Where(x => !x.IsDeleted && x.ID == ID)
                           .Select(x => new ShowVM()
                           {
                               ID = x.ID,
                               EventID = x.EventID,
                               TimeString = x.Time.ToString(@"hh\:mm"),
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at AddOrEdit (GET) in {_Title} Controller due to {ex}");
            }
            return View(_item);
        }
        //AddOrEdit (POST)
        [Authorize(Roles = "Super Admin,Admin")]
        [HttpPost]
        public async Task<IActionResult> AddOrEdit(ShowVM Model)
        {
            try
            {
                if (Model.EventID != 0)
                {
                    ViewBag.EventID = Model.EventID;
                }
                if (ModelState.IsValid)
                {
                    var _item = new Show();
                    if (TimeSpan.TryParse(Model.TimeString, out TimeSpan parsedTimeSpan))
                    {
                        Model.Time = parsedTimeSpan;
                    }
                    var _event = _context.Events.Where(x => !x.IsDeleted && x.ID == Model.EventID).FirstOrDefault();
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
                                    var _seatIDs = await _context.Seats.Include(x => x.Column)
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
        //Delete
        [Authorize(Roles = "Super Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int ID, int EventID)
        {
            try
            {
                var _item = _context.Shows.Where(x => !x.IsDeleted && x.ID == ID)
                                  .AsNoTracking().FirstOrDefault();
                if (_item != null)
                {
                    _item.IsDeleted = true;
                    _item.DeletedByUserID = _UserID;
                    _item.DeletedDateTime = DateTime.UtcNow.AddHours(5);
                    _context.ChangeTracker.Clear();
                    _context.Entry(_item).State = EntityState.Modified;
                    var _tickets = _context.Tickets.Where(x => !x.IsDeleted && x.ShowID == ID).ToList();
                    if (_tickets.Count > 0)
                    {
                        _context.Tickets.RemoveRange(_tickets);
                    }
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
            return RedirectToAction("Details", "Event", new { ID = EventID });
        }
        //Update Tickets (GET)
        [Authorize(Roles = "Super Admin,Admin")]
        public async Task<IActionResult> UpdateTickets(int ID, int EventID)
        {
            var _item = new TicketsBySeatVM();
            try
            {
                if (ID != 0 && EventID != 0)
                {
                    _item.ShowID = ID;
                    _item.EventID = EventID;
                }
                var _tickets = await _context.Tickets
                .Where(x => !x.IsDeleted && x.ShowID == ID)
                .Select(x => new
                {
                    x.ID,
                    x.CostPrice,
                    RowLabel = x.Seat.Column.Row.RowLabel,
                    x.IsReserved,
                })
                    .AsNoTracking()
                    .GroupBy(x => x.RowLabel)
                    .Select(g => new TicketRowVM
                    {
                        ID = g.First().ID,
                        RowLabel = g.Key,
                        Price = g.First().CostPrice,
                        IsReserved = g.First().IsReserved
                    })
                    .ToListAsync();
                _item.TicketRowList = _tickets;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at UpdateTickets (GET) in {_Title} Controller due to {ex}");
            }
            return View(_item);
        }
        //End
        [Authorize(Roles = "Super Admin,Admin")]
        [HttpPost]
        public async Task<IActionResult> End(int ID,int EventID)
        {
            try
            {
                var _item = _context.Shows.Where(x => !x.IsDeleted && x.ID == ID)
                                  .AsNoTracking().FirstOrDefault();
                if (_item != null)
                {
                    _item.IsEnded = true;
                    _item.UpdatedByUserID = _UserID;
                    _item.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                    _context.ChangeTracker.Clear();
                    _context.Entry(_item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"1,{_Title} Successfully Ended";
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
            return RedirectToAction("Details", "Event", new { ID = EventID });
        }
        //View Tickets (GET)
        [Authorize(Roles = "Super Admin,Admin,Moderator,User")]
        public IActionResult ViewTickets(int ID, int EventID)
        {
            // Get all rows with their columns from database
            try
            {
                if (EventID != 0)
                {
                    ViewBag.EventID = EventID;
                }
                var _object = _context.Tickets
                    .Include(x => x.Show)
                    .ThenInclude(x => x.Event)
                    .ThenInclude(x => x.Auditorium)
                    .ThenInclude(x => x.Rows)
                    .ThenInclude(x => x.Columns)
                    .Where(x => !x.IsDeleted && x.ID == ID).Select(x => new SeatVM()
                    {
                        AuditoriumName = x.Show.Event.Auditorium.Title.Trim(),
                        VenueName = x.Show.Event.Auditorium.Venue.Title.Trim(),
                        ShowDate = x.Show.Date,
                        Rows = x.Show.Event.Auditorium.Rows.Select(y => new RowVM()
                        {
                            RowLabel = y.RowLabel,
                            Columns = y.Columns.Select(c => new ColumnVM()
                            {
                                ID = c.ID,
                                ColumnNumber = c.ColumnNumber,
                                SeatTickets = c.Seats.Select(s => new TicketVM()
                                {
                                    CostPrice = x.CostPrice,
                                    SeatNumber = s.SeatNumber,
                                    IsReserved = x.IsReserved,
                                }).ToList()
                            }).ToList()
                        }).ToList()
                    }).FirstOrDefault();
                return View(_object ?? new SeatVM());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at UpdateSeats (GET) in {_Title} Controller due to {ex}");
            }
            return View(new SeatVM());
        }
        //Update Tickets (POST)
        [Authorize(Roles = "Super Admin,Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateTickets(TicketsBySeatVM Model)
        {
            try
            {
                // Load all tickets for the given show
                var tickets = await _context.Tickets
                    .Include(x => x.Seat)
                        .ThenInclude(x => x.Column)
                            .ThenInclude(x => x.Row)
                    .Where(x => !x.IsDeleted && x.ShowID == Model.ShowID)
                    .ToListAsync();

                foreach (var row in Model.TicketRowList)
                {
                    // Find matching tickets by row label
                    var matchingTickets = tickets
                        .Where(t => t.Seat?.Column?.Row?.RowLabel == row.RowLabel)
                        .ToList();
                    foreach (var ticket in matchingTickets)
                    {
                        // Apply updates
                        ticket.CostPrice = row.Price;
                        ticket.IsReserved = ticket.IsReserved;
                        ticket.UpdatedByUserID = _UserID;
                        ticket.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.Entry(ticket).State = EntityState.Modified;
                    }
                }
                await _context.SaveChangesAsync();
                TempData["Message"] = "1,Ticket Prices Successfully Updated";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at UpdateTickets (POST) in {_Title} Controller due to {ex}");
            }
            return RedirectToAction("UpdateTickets", _Title, new { ID = Model.ShowID, EventID = Model.EventID });
        }
    }
}
