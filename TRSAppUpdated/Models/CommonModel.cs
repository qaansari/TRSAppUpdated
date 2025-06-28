using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class CommonModel
    {
        public bool IsDeleted { get; set; }
        [ForeignKey("CreatedByUser")]
        [Column(TypeName = "varchar(70)")]
        public string CreatedByUserID { get; set; }
        [ForeignKey("UpdatedByUser")]
        [Column(TypeName = "varchar(70)")]
        public string? UpdatedByUserID { get; set; }
        [ForeignKey("DeletedByUser")]
        [Column(TypeName = "varchar(70)")]
        public string? DeletedByUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public User CreatedByUser { get; set; }
        public User? UpdatedByUser { get; set; }
        public User? DeletedByUser { get; set; }
    }
}
