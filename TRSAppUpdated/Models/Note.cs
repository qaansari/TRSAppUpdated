using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class Note : CommonModel
    {
        [Key]
        public int ID { get; set; }
        public int Seq { get; set; }
        [Column(TypeName = "text")]
        public string EnTitle { get; set; }
        [Column(TypeName = "text")]
        public string UrduTitle { get; set; }
        [ForeignKey("NoteType")]
        public int NoteTypeID { get; set; }
        public NoteType NoteType { get; set; }
    }
}
