using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string FirstName { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string LastName { get; set; }
        [Column(TypeName = "varchar(20)")]
        public string ContactNo { get; set; }
        [Column(TypeName = "varchar(320)")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Column(TypeName = "varchar(max)")]
        public string Password { get; set; }
        [ForeignKey("Role")]
        [Column(TypeName = "varchar(70)")]
        public int RoleID { get; set; }
        [Column(TypeName = "varchar(300)")]
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey("CreatedByUser")]
        [Column(TypeName = "varchar(70)")]
        public int? CreatedByUserID { get; set; }
        [ForeignKey("UpdatedByUser")]
        [Column(TypeName = "varchar(70)")]
        public int? UpdatedByUserID { get; set; }
        [ForeignKey("DeletedByUser")]
        [Column(TypeName = "varchar(70)")]
        public int? DeletedByUserID { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public User? CreatedByUser { get; set; }
        public User? UpdatedByUser { get; set; }
        public User? DeletedByUser { get; set; }
        public Role Role { get; set; }
    }
}
