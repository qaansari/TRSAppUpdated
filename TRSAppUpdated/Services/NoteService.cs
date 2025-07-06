using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class NoteService : INoteService
    {

        private readonly AppDbContext _context;
        private readonly ILogger<NoteService> _logger;
        private readonly string _Title = "Note";
        public NoteService(AppDbContext context, ILogger<NoteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<NoteVM> GetNotesByNoteTypeID(int noteTypeID)
        {
            try
            {
                var _notes = _context.Notes.Where(x => !x.IsDeleted && x.NoteTypeID == noteTypeID).Select(x => new NoteVM()
                {
                    ID = x.ID,
                    Seq = x.Seq,
                    EnTitle = x.EnTitle,
                    UrduTitle = x.UrduTitle,
                    CreatedByUserID = x.CreatedByUserID,
                    CreatedDateTime = x.CreatedDateTime,
                }).OrderBy(x => x.Seq).AsNoTracking().ToList();
                return _notes ?? new List<NoteVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetNotesByNoteTypeID in {_Title} Service due to {ex}");
            }
            return new List<NoteVM>();
        }
    }
}
