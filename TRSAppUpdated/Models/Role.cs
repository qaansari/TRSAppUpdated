using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class Role
    {
        [Key]
        [Column(TypeName = "varchar(70)")]
        public string ID { get; set; }
        [Column(TypeName="varchar(20)")]
        public string Title { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
