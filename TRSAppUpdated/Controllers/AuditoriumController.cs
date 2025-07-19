using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.Services;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    [Authorize(Roles = "Super Admin")]
    public class AuditoriumController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AuditoriumController> _logger;
        private readonly IVenueService _venueService;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;
        private readonly string _Title = "Auditorium";
        private readonly int _UserID;
        public AuditoriumController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment, ILogger<AuditoriumController> logger,
            IVenueService venueService, ICountryService countryService,
            ICityService cityService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _venueService = venueService;
            _countryService = countryService;
            _cityService = cityService;
            _UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("UserID"));
        }
        //Index (Get All)
        public async Task<IActionResult> Index(int _pageNo = 1, int _pageSize = 10, string _searchedTerm = "")
        {
            try
            {
                var (_result, totalRecords) = await GetList(_pageNo, _pageSize, _searchedTerm);
                ViewBag.TotalRecords = totalRecords;
                ViewBag.PageNo = _pageNo;
                ViewBag.PageSize = _pageSize;
                ViewBag.SearchedTerm = _searchedTerm;
                return View(_result ?? new List<AuditoriumVM>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index in {_Title} Controller due to {ex}");
            }
            return View(new List<AuditoriumVM>());
        }
        // GetPartial
        [HttpGet]
        public async Task<IActionResult> GetPartial(int _pageNo = 1, int _pageSize = 10, string _searchedTerm = "")
        {
            var _partialViewName = _Title.Contains(" ") ? _Title.Replace(" ", "") : _Title;
            try
            {
                var (_result, totalRecords) = await GetList(_pageNo, _pageSize, _searchedTerm);
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
            return PartialView($"_{_partialViewName}TablePartial", new List<AuditoriumVM>());
        }
        //AddOrEdit (GET)
        public IActionResult AddOrEdit(int ID = 0)
        {
            var _item = new AuditoriumVM();
            try
            {
                ViewData["CountryList"] = _countryService.GetAll();
                ViewData["CityList"] = _cityService.GetAll();
                ViewData["VenueList"] = _venueService.GetAll();
                if (ID != 0)
                {
                    _item = _context.Auditoriums.Where(x => !x.IsDeleted && x.ID == ID)
                        .Select(x => new AuditoriumVM()
                        {
                            ID = x.ID,
                            Title = x.Title,
                            CountryID = x.Venue.City.CountryID,
                            CityID = x.Venue.CityID,
                            VenueID = x.VenueID,
                            NoOfRows = x.NoOfRows,
                            StartingRow = x.StartingRow,
                            Alignment=x.Alignment,
                            ImagePath = x.ImagePath,
                            CreatedByUserID = x.CreatedByUserID,
                            CreatedDateTime = x.CreatedDateTime,
                        })
                        .AsNoTracking().FirstOrDefault();
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
        public async Task<IActionResult> AddOrEdit(AuditoriumVM Model)
        {
            try
            {
                ViewData["CountryList"] = _countryService.GetAll();
                ViewData["CityList"] = _cityService.GetAll();
                ViewData["VenueList"] = _venueService.GetAll();
                if (ModelState.IsValid)
                {
                    var _item = new Auditorium();

                    if (Model.ID != 0)
                    {
                        var isExists = _context.Auditoriums.Where(x => !x.IsDeleted
                            && x.ID != Model.ID
                            && x.Title.Trim().ToLower() == Model.Title.Trim().ToLower())
                        .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item = _context.Auditoriums.Where(x => !x.IsDeleted && x.ID == Model.ID)
                            .AsNoTracking().FirstOrDefault();
                        if (_item != null)
                        {
                            if (_item.NoOfRows != Model.NoOfRows || _item.StartingRow.Trim() != Model.StartingRow.Trim())
                            {
                                var _rows = _context.Rows.Include(x => x.Columns)
                                    .ThenInclude(x => x.Seats)
                                    .Where(x => x.AuditoriumID == _item.ID).ToList();
                                if (_rows.Count > 0)
                                {
                                    var _columns = _rows.SelectMany(r => r.Columns).ToList();
                                    if (_columns.Count > 0)
                                    {
                                        var _seats = _columns.SelectMany(r => r.Seats).ToList();
                                        if (_seats.Count > 0)
                                        {
                                            _context.Seats.RemoveRange(_seats);
                                        }
                                        _context.Columns.RemoveRange(_columns);
                                    }
                                    _context.Rows.RemoveRange(_rows);
                                }
                                //Rows Insertion Start
                                char _startChar = char.ToUpper(Model.StartingRow[0]);
                                int _startAscii = Convert.ToInt32(_startChar);
                                var _rowsToAdd = new List<Row>();
                                for (int i = 0; i < Model.NoOfRows; i++)
                                {
                                    char _rowLetter = Convert.ToChar(_startAscii + i);
                                    _rowsToAdd.Add(new Row()
                                    {
                                        AuditoriumID = _item.ID,
                                        RowLabel = _rowLetter.ToString(),
                                        CreatedByUserID = _UserID,
                                        CreatedDateTime = DateTime.UtcNow.AddHours(5),
                                    });
                                }
                                _context.Rows.AddRange(_rowsToAdd);
                                //Rows Insertion End
                            }
                            _item.Title = Model.Title.Trim();
                            _item.VenueID = Model.VenueID;
                            _item.NoOfRows = Model.NoOfRows;
                            _item.StartingRow = Model.StartingRow.Trim();
                            _item.Alignment = Model.Alignment.Trim();
                            _item.UpdatedByUserID = _UserID;
                            _item.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                            _context.Entry(_item).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                            TempData["Message"] = $"1,{_Title} Successfully Updated";
                        }
                    }
                    else
                    {
                        var isExists = _context.Auditoriums.Where(x => !x.IsDeleted
                            && x.Title.Trim().ToLower() == Model.Title.Trim().ToLower())
                        .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item.Title = Model.Title.Trim();
                        _item.VenueID = Model.VenueID;
                        _item.NoOfRows = Model.NoOfRows;
                        _item.StartingRow = Model.StartingRow.Trim();
                        _item.Alignment = Model.Alignment.Trim();
                        _item.CreatedByUserID = _UserID;
                        _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.Auditoriums.Add(_item);
                        await _context.SaveChangesAsync();
                        //Rows Insertion Start
                        char _startChar = char.ToUpper(_item.StartingRow[0]);
                        int _startAscii = Convert.ToInt32(_startChar);
                        var _rowsToAdd = new List<Row>();
                        for (int i = 0; i < Model.NoOfRows; i++)
                        {
                            char _rowLetter = Convert.ToChar(_startAscii + i);
                            _rowsToAdd.Add(new Row()
                            {
                                AuditoriumID = _item.ID,
                                RowLabel = _rowLetter.ToString(),
                                CreatedByUserID = _UserID,
                                CreatedDateTime = DateTime.UtcNow.AddHours(5),
                            });
                        }
                        _context.Rows.AddRange(_rowsToAdd);
                        //Rows Insertion End
                        await _context.SaveChangesAsync();
                        TempData["Message"] = $"1,{_Title} Successfully Added";
                    }
                    if (_item != null && Model.ImageFile != null)
                    {
                        var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "auditoriums");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        var _fileName = $"auditorium" + "_" + _item.CreatedDateTime.ToString("yyyyddMMhhmmss") + Path.GetExtension(Model.ImageFile.FileName);
                        _item.ImagePath = _fileName.Trim();
                        _item.UpdatedByUserID = _UserID;
                        _item.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                        var _filePath = Path.Combine(folderPath, _fileName);
                        using (var _stream = new FileStream(_filePath, FileMode.Create))
                        {
                            await Model.ImageFile.CopyToAsync(_stream);
                        }
                        _context.ChangeTracker.Clear();
                        _context.Entry(_item).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
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
        //Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int ID)
        {
            try
            {
                var _item = _context.Auditoriums.Where(x => !x.IsDeleted && x.ID == ID)
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
            return RedirectToAction("Index");
        }
        //Get List
        [NonAction]
        public async Task<(List<AuditoriumVM> _result, int totalRecords)> GetList(int _pageNo, int _pageSize, string _searchedTerm)
        {

            var _result = new List<AuditoriumVM>();
            int totalRecords = 0;
            try
            {
                // For AJAX calls
                var _query = _context.Auditoriums
                    .Where(x => !x.IsDeleted)
                           .Select(x => new AuditoriumVM()
                           {
                               ID = x.ID,
                               Title = x.Title,
                               ImagePath = x.ImagePath,
                               VenueTitle = x.Venue.Title,
                               StartingRow = x.StartingRow,
                               NoOfRows = x.NoOfRows,
                               VenueImagePath = x.Venue.ImagePath
                           })
                           .AsNoTracking()
                           .AsQueryable();
                if (!string.IsNullOrEmpty(_searchedTerm))
                {
                    _query = _query
                        .Where(x => x.Title.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                    || (!string.IsNullOrEmpty(x.VenueTitle) && x.VenueTitle.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())));
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
        //Update Columns (GET)
        public IActionResult UpdateColumns(int AuditoriumId)
        {
            try
            {
                var _rows = _context.Rows
                        .Where(x => x.AuditoriumID == AuditoriumId)
                        .Select(x => new RowWithColumnVM
                        {
                            RowID = x.ID,
                            RowLabel = x.RowLabel,
                            ColumnCount = _context.Columns.Count(y => y.RowID == x.ID)
                        })
                        .ToList();
                return View(_rows ?? new List<RowWithColumnVM>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at UpdateColumns (GET) in {_Title} Controller due to {ex}");
            }
            return View(new List<RowWithColumnVM>());
        }
        //Update Columns (POST)
        [HttpPost]
        public async Task<IActionResult> UpdateColumns(List<RowWithColumnVM> model)
        {
            try
            {
                foreach (var item in model)
                {
                    // Get the row object including its label
                    var row = _context.Rows.FirstOrDefault(r => r.ID == item.RowID);
                    if (row == null)
                    {
                        continue;
                    }
                    var existingColumns = _context.Columns
                        .Where(c => c.RowID == row.ID)
                        .OrderBy(c => c.ColumnNumber)
                        .ToList();
                    int existingCount = existingColumns.Count;
                    // Delete extra columns if any
                    if (existingCount > item.ColumnCount)
                    {
                        var toRemove = existingColumns.Skip(item.ColumnCount).ToList();
                        _context.Columns.RemoveRange(toRemove);
                    }
                    // Add new columns if needed
                    else if (existingCount < item.ColumnCount)
                    {
                        for (int i = existingCount + 1; i <= item.ColumnCount; i++)
                        {
                            _context.Columns.Add(new Column
                            {
                                RowID = row.ID,
                                ColumnNumber = i,
                                CreatedByUserID = _UserID,
                                CreatedDateTime = DateTime.UtcNow.AddHours(5),
                            });
                        }
                    }
                }
                await _context.SaveChangesAsync();
                TempData["Message"] = $"1,Columns Successfully Updated";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at UpdateColumns (POST) in {_Title} Controller due to {ex}");
            }
            return RedirectToAction("Index"); // or your desired route
        }
        //View Seats (GET)
        public IActionResult ViewSeats(int AuditoriumId)
        {
            // Get all rows with their columns from database
            try
            {
                var _object = _context.Auditoriums
                    .Include(x => x.Venue)
                    .Include(x => x.Rows)
                    .ThenInclude(x => x.Columns)
                    .Where(x => !x.IsDeleted && x.ID == AuditoriumId).Select(x => new SeatVM()
                    {
                        AuditoriumName = x.Title.Trim(),
                        VenueName = x.Venue.Title.Trim(),
                        AuditoriumAlignment=x.Alignment,
                        Rows = x.Rows.Select(y => new RowVM()
                        {
                            RowLabel = y.RowLabel,
                            Columns = y.Columns.Select(c => new ColumnVM()
                            {
                                ID = c.ID,
                                ColumnNumber = c.ColumnNumber,
                                Seats = c.Seats.Select(s => s.SeatNumber).ToList()
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
        //Update Seats (GET)
        public IActionResult UpdateSeats(int AuditoriumId)
        {
            // Get all rows with their columns from database
            try
            {
                var rows = _context.Rows.Include(r => r.Columns).ThenInclude(x => x.Seats)
                    .Where(r => r.AuditoriumID == AuditoriumId).ToList();
                var _list = rows.Select(r => new RowSeatInputVM
                {
                    RowID = r.ID,
                    RowLabel = r.RowLabel,
                    Columns = r.Columns.OrderBy(c => c.ColumnNumber)
                                      .Select(c => new ColumnSeatInputVM
                                      {
                                          ColumnID = c.ID,
                                          ColumnNumber = c.ColumnNumber,
                                          SeatCount = c.Seats.Count
                                      }).ToList()
                }).ToList();
                return View(_list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at UpdateSeats (GET) in {_Title} Controller due to {ex}");
            }
            return View(new List<RowSeatInputVM>());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSeats(List<RowSeatInputVM> model)
        {
            try
            {
                foreach (var row in model)
                {
                    int _counter = 1;
                    foreach (var column in row.Columns)
                    {
                        // Fetch existing seats for the column
                        var existingSeats = _context.Seats
                            .Where(s => s.ColumnID == column.ColumnID)
                            .ToList();
                        if (existingSeats.Count > 0)
                        {
                            _context.Seats.RemoveRange(existingSeats);
                        }
                        // Add new seats
                        for (int i = 1; i <= column.SeatCount; i++)
                        {
                            _context.Seats.Add(new Seat
                            {
                                ColumnID = column.ColumnID,
                                SeatNumber = _counter,
                                CreatedByUserID = _UserID,
                                CreatedDateTime = DateTime.UtcNow.AddHours(5),
                                // Add any other necessary properties here
                            });
                            _counter++;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                TempData["Message"] = $"1,Seats Successfully Updated";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at UpdateSeats (POST) in {_Title} Controller due to {ex}");
            }
            return RedirectToAction("Index");
        }

    }
}
