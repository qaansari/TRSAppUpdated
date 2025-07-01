using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class CommonModel
    {
        public bool IsDeleted { get; set; } = false;
        [ForeignKey("CreatedByUser")]
        public int CreatedByUserID { get; set; }
        [ForeignKey("UpdatedByUser")]
        public int? UpdatedByUserID { get; set; }
        [ForeignKey("DeletedByUser")]
        public int? DeletedByUserID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public User CreatedByUser { get; set; }
        public User? UpdatedByUser { get; set; }
        public User? DeletedByUser { get; set; }
    }
}
