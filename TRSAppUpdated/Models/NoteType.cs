using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.Models
{
    public class NoteType : CommonModel
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string Title { get; set; }
        public ICollection<Note> Notes { get; set; }
    }
}
