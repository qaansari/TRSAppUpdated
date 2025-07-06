using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Layouts;
using System.Security.Claims;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Controllers
{
    [Authorize(Roles = "Super Admin")]
    public class NoteTypeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<NoteTypeController> _logger;
        private readonly INoteService _noteService;
        private readonly string _Title = "Note Type";
        private readonly int _UserID;
        public NoteTypeController(AppDbContext context, IHttpContextAccessor httpContextAccessor,ILogger<NoteTypeController> logger,INoteService noteService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _noteService = noteService;
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
                return View(_result ?? new List<NoteTypeVM>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at Index in {_Title} Controller due to {ex}");
            }
            return View(new List<NoteTypeVM>());
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
            return PartialView($"_{_partialViewName}TablePartial", new List<NoteTypeVM>());
        }
        //AddOrEdit (GET)
        public IActionResult AddOrEdit(int ID = 0)
        {
            var _item = new NoteTypeVM();
            try
            {
                if (ID != 0)
                {
                    _item = _context.NoteTypes.Where(x => !x.IsDeleted && x.ID == ID)
                        .Select(x => new NoteTypeVM()
                        {
                            ID = x.ID,
                            Title = x.Title,
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
        public async Task<IActionResult> AddOrEdit(NoteTypeVM Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _item = new NoteType();
                    if (Model.ID != 0)
                    {
                        var isExists = _context.NoteTypes.Where(x => !x.IsDeleted
                            && x.ID != Model.ID
                            && x.Title.Trim().ToLower() == Model.Title.Trim().ToLower())
                        .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item = _context.NoteTypes.Where(x => !x.IsDeleted && x.ID == Model.ID)
                            .AsNoTracking().FirstOrDefault();
                        if (_item != null)
                        {
                            _item.Title = Model.Title.Trim();
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
                        var isExists = _context.NoteTypes.Where(x => !x.IsDeleted
                            && x.Title.Trim().ToLower() == Model.Title.Trim().ToLower())
                        .Any();
                        if (isExists)
                        {
                            TempData["Message"] = $"0,{_Title} Already Exists";
                            return View(Model);
                        }
                        _item.Title = Model.Title.Trim();
                        _item.CreatedByUserID = _UserID;
                        _item.CreatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.NoteTypes.Add(_item);
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
                var _item = _context.NoteTypes.Where(x => !x.IsDeleted && x.ID == ID)
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
        public async Task<(List<NoteTypeVM> _result, int totalRecords)> GetList(int _pageNo, int _pageSize, string _searchedTerm)
        {

            var _result = new List<NoteTypeVM>();
            int totalRecords = 0;
            try
            {
                // For AJAX calls
                var _query = _context.NoteTypes.Where(x => !x.IsDeleted)
                           .Select(x => new NoteTypeVM()
                           {
                               ID = x.ID,
                               Title = x.Title,
                           })
                           .AsNoTracking()
                           .AsQueryable();
                if (!string.IsNullOrEmpty(_searchedTerm))
                {
                    _query = _query
                        .Where(x => x.Title.Trim().ToLower().Contains(_searchedTerm.Trim().ToLower()));
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
        //Update Notes (GET)
        public IActionResult UpdateNotes(int noteTypeID)
        {
            try
            {
                var _notes = _noteService.GetNotesByNoteTypeID(noteTypeID);
                return View(_notes ?? new List<NoteVM>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at UpdateNotes in {_Title} Controller due to {ex}");
            }
            return View(new List<NoteVM>());
        }
        [HttpPost]
        public async Task<IActionResult> UpdateNotes(List<Note> notes)
        {
            try
            {
                foreach (var note in notes)
                {
                    var existingNote = await _context.Notes.Where(x => !x.IsDeleted && x.ID == note.ID).FirstOrDefaultAsync();
                    if (existingNote != null)
                    {
                        existingNote.Seq = note.Seq;
                        existingNote.UpdatedByUserID = _UserID;
                        existingNote.UpdatedDateTime = DateTime.UtcNow.AddHours(5);
                        _context.Entry(existingNote).State = EntityState.Modified;
                    }
                }
                await _context.SaveChangesAsync();
                TempData["Message"]="1,Notes Sequence Updated Successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at UpdateNotes (POST) in {_Title} Controller due to {ex}");
            }
            return RedirectToAction("Index");
        }
    }
}
