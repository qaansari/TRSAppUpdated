using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    [Authorize]
    public class ShowController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ShowController> _logger;
        private readonly ICacheService _cacheService;
        private readonly string _Title = "Show";
        private readonly int _UserID;
        public ShowController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            ILogger<ShowController> logger, ICacheService cacheService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _cacheService = cacheService;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
        }
        [Authorize(Roles = "Super Admin")]
        public IActionResult AddOrEdit(int ID = 0, int EventID = 0)
        {
            var _item = new ShowVM();
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
            return View(_item);
        }
        //AddOrEdit (POST)
        [Authorize(Roles = "Super Admin")]
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
                                    _item.UpdatedDateTime = AppHelper.NowPakistan;
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
                                _item.CreatedDateTime = AppHelper.NowPakistan;
                                _context.Shows.Add(_item);
                                await _context.SaveChangesAsync();

                                TempData["Message"] = $"1,{_Title} Successfully Added";
                            }
                            var isTicketExists = _context.Tickets.Where(x => !x.IsDeleted && x.ShowID == _item.ID).Any();
                            if (!isTicketExists)
                            {
                                var _seatIDs = await _context.Seats
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
                                    Category = string.Empty,
                                    CreatedByUserID = _UserID,
                                    CreatedDateTime = AppHelper.NowPakistan,
                                }).ToList();
                                await _context.Tickets.AddRangeAsync(_tickets);
                                await _context.SaveChangesAsync();
                            }
                            _cacheService.Remove(CacheKeys.ShowsList);
                            _cacheService.Remove(CacheKeys.ViewTickets(_item.ID, Model.EventID));
                            _cacheService.Remove(CacheKeys.EventDetails(Model.EventID));
                            _cacheService.Remove(CacheKeys.DashboardMetrics);
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
                    _item.DeletedDateTime = AppHelper.NowPakistan;
                    _context.ChangeTracker.Clear();
                    _context.Entry(_item).State = EntityState.Modified;
                    var _tickets = _context.Tickets.Where(x => !x.IsDeleted && x.ShowID == ID).ToList();
                    if (_tickets.Count > 0)
                    {
                        _context.Tickets.RemoveRange(_tickets);
                    }
                    await _context.SaveChangesAsync();
                    _cacheService.Remove(CacheKeys.ShowsList);
                    _cacheService.Remove(CacheKeys.ViewTickets(ID, EventID));
                    _cacheService.Remove(CacheKeys.EventDetails(EventID));
                    _cacheService.Remove(CacheKeys.DashboardMetrics);
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
        //End
        [Authorize(Roles = "Super Admin,Admin")]
        [HttpPost]
        public async Task<IActionResult> End(int ID, int EventID)
        {
            try
            {
                var _item = _context.Shows.Where(x => !x.IsDeleted && x.ID == ID)
                                  .AsNoTracking().FirstOrDefault();
                if (_item != null)
                {
                    _item.IsEnded = true;
                    _item.UpdatedByUserID = _UserID;
                    _item.UpdatedDateTime = AppHelper.NowPakistan;
                    _context.ChangeTracker.Clear();
                    _context.Entry(_item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    _cacheService.Remove(CacheKeys.ShowsList);
                    _cacheService.Remove(CacheKeys.ViewTickets(ID, EventID));
                    _cacheService.Remove(CacheKeys.EventDetails(EventID));
                    TempData["Message"] = $"1,{_Title} Successfully Ended";
                }
                else
                {
                    TempData["Message"] = $"0,No {_Title} Found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at End (POST) in {_Title} Controller due to {ex}");
            }
            return RedirectToAction("Details", "Event", new { ID = EventID });
        }
        //Update Tickets (GET)
        [Authorize(Roles = "Super Admin")]
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
                var rawTickets = await _context.Tickets
                    .Where(x => !x.IsDeleted && x.ShowID == ID)
                    .Select(x => new
                    {
                        x.ID,
                        x.CostPrice,
                        RowLabel = x.Seat.Column.Row.RowLabel,
                        x.Category,
                        x.IsReserved,
                    })
                    .AsNoTracking()
                    .ToListAsync();

                var _tickets = rawTickets
                    .GroupBy(x => x.RowLabel)
                    .Select(g => new TicketRowVM
                    {
                        ID = g.First().ID,
                        RowLabel = g.Key,
                        Price = g.First().CostPrice,
                        Category = g.First().Category,
                        IsReserved = g.First().IsReserved
                    })
                    .OrderBy(x => x.RowLabel)
                    .ToList();

                _item.TicketRowList = _tickets;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at UpdateTickets (GET) in {_Title} Controller due to {ex}");
            }
            return View(_item);
        }
        //Update Tickets (POST)
        [Authorize(Roles = "Super Admin")]
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
                        ticket.Category = row.Category;
                        ticket.IsRefunded = row.IsRefunded;
                        ticket.IsReserved = row.IsReserved;
                        ticket.UpdatedByUserID = _UserID;
                        ticket.UpdatedDateTime = AppHelper.NowPakistan;
                        _context.Entry(ticket).State = EntityState.Modified;
                    }
                }
                await _context.SaveChangesAsync();
                _cacheService.Remove(CacheKeys.ViewTickets(Model.ShowID, Model.EventID));
                _cacheService.Remove(CacheKeys.EventDetails(Model.EventID));
                TempData["Message"] = "1,Ticket Prices Successfully Updated";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at UpdateTickets (POST) in {_Title} Controller due to {ex}");
            }
            return RedirectToAction("UpdateTickets", _Title, new { ID = Model.ShowID, EventID = Model.EventID });
        }
        //View Tickets (GET) - Optimized with In-Memory Caching & Flat Ticket Query
        [Authorize(Roles = "Super Admin,Admin,Moderator,User")]
        public async Task<IActionResult> ViewTickets(int ID, int EventID)
        {
            try
            {
                if (EventID != 0)
                {
                    ViewBag.EventID = EventID;
                }

                var cacheKey = CacheKeys.ViewTickets(ID, EventID);
                var seatVM = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var show = await _context.Shows
                        .Include(s => s.Event)
                            .ThenInclude(e => e.Auditorium)
                                .ThenInclude(a => a.Venue)
                        .Include(s => s.Event)
                            .ThenInclude(e => e.Auditorium)
                                .ThenInclude(a => a.Rows)
                                    .ThenInclude(r => r.Columns)
                                        .ThenInclude(c => c.Seats)
                        .Where(s => !s.IsDeleted && s.ID == ID && s.EventID == EventID)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (show == null || show.Event?.Auditorium == null)
                    {
                        return new SeatVM();
                    }

                    var tickets = await _context.Tickets
                        .Where(t => !t.IsDeleted && t.ShowID == ID)
                        .Select(t => new
                        {
                            t.SeatID,
                            t.CostPrice,
                            t.SalePrice,
                            t.IsReserved,
                            t.Category
                        })
                        .AsNoTracking()
                        .ToListAsync();

                    // Group safely by SeatID to avoid duplicate key exceptions
                    var ticketBySeatId = tickets
                        .GroupBy(t => t.SeatID)
                        .ToDictionary(g => g.Key, g => g.First());

                    var aud = show.Event.Auditorium;
                    return new SeatVM
                    {
                        AuditoriumName = aud.Title?.Trim(),
                        EventName = show.Event.Title?.Trim(),
                        VenueName = aud.Venue?.Title?.Trim(),
                        ShowDate = show.Date,
                        ReservedSeats = tickets.Count(x => x.IsReserved),
                        EstimatedRevenue = tickets.Sum(x => x.CostPrice),
                        AmountReceived = tickets.Where(x => x.IsReserved).Sum(x => x.SalePrice),
                        AuditoriumAlignment = aud.Alignment?.Trim(),
                        Rows = aud.Rows.OrderBy(row => row.ID).Select(row =>
                        {
                            var firstSeatInRow = row.Columns.SelectMany(c => c.Seats).FirstOrDefault();
                            ticketBySeatId.TryGetValue(firstSeatInRow?.ID ?? 0, out var rowTicket);
                            var rowPrice = rowTicket?.CostPrice ?? 0;

                            return new RowVM
                            {
                                RowLabel = row.RowLabel,
                                CostPrice = rowPrice.ToString(),
                                Category = rowTicket?.Category,
                                Columns = row.Columns.OrderBy(col => col.ColumnNumber).Select(col => new ColumnVM
                                {
                                    ID = col.ID,
                                    ColumnNumber = col.ColumnNumber,
                                    SeatTickets = col.Seats.OrderBy(seat => seat.SeatNumber).Select(seat =>
                                    {
                                        ticketBySeatId.TryGetValue(seat.ID, out var ticket);
                                        return new TicketVM
                                        {
                                            SeatNumber = seat.SeatNumber,
                                            IsReserved = ticket?.IsReserved ?? false
                                        };
                                    }).ToList()
                                }).ToList()
                            };
                        }).ToList()
                    };
                }, TimeSpan.FromMinutes(15), TimeSpan.FromHours(1));

                return View(seatVM ?? new SeatVM());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at ViewTickets in {_Title} Controller due to {ex}");
            }
            return View(new SeatVM());
        }
    }
}
