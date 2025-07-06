using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface INoteService
    {
        List<NoteVM> GetNotesByNoteTypeID(int noteTypeID);
    }
}
