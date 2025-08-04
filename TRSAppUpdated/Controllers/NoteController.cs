using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    [Authorize(Roles = "Super Admin")]
    public class NoteController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<NoteController> _logger;
        private readonly INoteTypeService _noteTypeService;
        private readonly string _Title = "Note";
        private readonly int _UserID;
        public NoteController(AppDbContext context, IHttpContextAccessor httpContextAccessor,
            ILogger<NoteController> logger,
            INoteTypeService noteTypeService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _noteTypeService = noteTypeService;
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
                return View(_result ?? new List<NoteVM>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index in {_Title} Controller due to {ex}");
            }
            return View(new List<NoteVM>());
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
            return PartialView($"_{_partialViewName}TablePartial", new List<NoteVM>());
        }
        //AddOrEdit (GET)
        public IActionResult AddOrEdit(int ID = 0)
        {
            var _item = new NoteVM();
            try
            {
                ViewData["NoteTypeList"] = _noteTypeService.GetAll();
                if (ID != 0)
                {
                    _item = _context.Notes.Where(x => !x.IsDeleted && x.ID == ID)
                        .Select(x => new NoteVM()
                        {
                            ID = x.ID,
                            EnTitle = x.EnTitle,
                            UrduTitle = x.UrduTitle,
                            Seq = x.Seq,
                            NoteTypeID = x.NoteTypeID,
                            CreatedByUserID = x.CreatedByUserID,
                            CreatedDateTime = x.CreatedDateTime,
                        })
                        .AsNoTracking().FirstOrDefault();
                }
                else
                {
                    _item.Seq = 1;
                    if (_context.Notes.Where(x => !x.IsDeleted).AsNoTracking().Any())
                    {
                        _item.Seq = (_context.Notes.Max(x => x.Seq)) + 1;
                    }
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
        public async Task<IActionResult> AddOrEdit(NoteVM Model)
        {
            try
            {
                ViewData["NoteTypeList"] = _noteTypeService.GetAll();
                if (ModelState.IsValid)
                {
                    var _item = new Note();
                    if (Model.ID != 0)
                    {
                        var isExists = _context.Notes.Where(x => !x.IsDeleted
                            && x.ID != Model.ID
                            && x.Seq == Model.Seq)
                        .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item = _context.Notes.Where(x => !x.IsDeleted && x.ID == Model.ID)
                            .AsNoTracking().FirstOrDefault();
                        if (_item != null)
                        {
                            _item.Seq = Model.Seq;
                            _item.EnTitle = Model.EnTitle.Trim();
                            _item.UrduTitle = Model.UrduTitle.Trim();
                            _item.NoteTypeID = Model.NoteTypeID;
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
                        var isExists = _context.Notes.Where(x => !x.IsDeleted
                              && x.Seq == Model.Seq).Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item.Seq = Model.Seq;
                        _item.EnTitle = Model.EnTitle.Trim();
                        _item.UrduTitle = Model.UrduTitle.Trim();
                        _item.NoteTypeID = Model.NoteTypeID;
                        _item.CreatedByUserID = _UserID;
                        _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.Notes.Add(_item);
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
        public async Task<IActionResult> Delete(int ID)
        {
            try
            {
                var _item = _context.Notes.Where(x => !x.IsDeleted && x.ID == ID)
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
        public async Task<(List<NoteVM> _result, int totalRecords)> GetList(int _pageNo, int _pageSize, string _searchedTerm)
        {

            var _result = new List<NoteVM>();
            int totalRecords = 0;
            try
            {
                // For AJAX calls
                var _query = _context.Notes.Where(x => !x.IsDeleted)
                           .Select(x => new NoteVM()
                           {
                               ID = x.ID,
                               EnTitle = x.EnTitle,
                               Seq = x.Seq,
                               UrduTitle = x.UrduTitle,
                               NoteTypeTitle = x.NoteType.Title
                           })
                           .AsNoTracking()
                           .AsQueryable();
                if (!string.IsNullOrEmpty(_searchedTerm))
                {
                    _query = _query
                        .Where(x => x.NoteTypeTitle.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                        || x.EnTitle.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                        || x.UrduTitle.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower())
                        || x.Seq.ToString().Trim().ToLower().Contains(_searchedTerm.Trim().ToLower()));
                }
                int _skip = (_pageNo - 1) * _pageSize;
                totalRecords = await _query.CountAsync();
                _query = _query.Skip(_skip).Take(_pageSize);
                _result = await _query.OrderBy(x => x.Seq).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetList in {_Title} Controller due to {ex}");
            }
            return (_result, totalRecords);
        }
    }
}
