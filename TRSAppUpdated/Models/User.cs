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
        public int? CreatedByUserID { get; set; }
        [ForeignKey("UpdatedByUser")]
        public int? UpdatedByUserID { get; set; }
        [ForeignKey("DeletedByUser")]
        public int? DeletedByUserID { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public User? UserCreatedByUser { get; set; }
        public User? UserUpdatedByUser { get; set; }
        public User? UserDeletedByUser { get; set; }
        public Role Role { get; set; }
        //Inverse props
        [InverseProperty("UserCreatedByUser")]
        public ICollection<User> UsersCreated { get; set; }
        [InverseProperty("UserUpdatedByUser")]
        public ICollection<User> UsersUpdated { get; set; }
        [InverseProperty("UserDeletedByUser")]
        public ICollection<User> UsersDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Auditorium> AuditoriumsCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Auditorium> AuditoriumsUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Auditorium> AuditoriumsDeleted { get; set; }
        [InverseProperty("CreatedByUser")]
        public ICollection<Category> CategoriesCreated { get; set; }
        [InverseProperty("UpdatedByUser")]
        public ICollection<Category> CategoriesUpdated { get; set; }
        [InverseProperty("DeletedByUser")]
        public ICollection<Category> CategoriesDeleted { get; set; }
    }
}
