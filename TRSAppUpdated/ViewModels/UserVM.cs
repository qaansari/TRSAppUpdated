using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRSAppUpdated.ViewModels
{
    public class UserVM
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(4, ErrorMessage = "Please fill out minimum 4 characters")]
        [StringLength(20, ErrorMessage = "Please fill out maximum 20 characters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(4, ErrorMessage = "Please fill out minimum 4 characters")]
        [StringLength(20, ErrorMessage = "Please fill out maximum 20 characters")]
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(11, ErrorMessage = "Please fill out minimum 11 characters")]
        [StringLength(20, ErrorMessage = "Please fill out maximum 20 characters")]
        public string ContactNumber { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(11, ErrorMessage = "Please fill out minimum 11 characters")]
        [StringLength(320, ErrorMessage = "Please fill out maximum 320 characters")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Please enter a valid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public string Password { get; set; }
        [Range(1,int.MaxValue,ErrorMessage = "Please fill out this field")]
        public int RoleID { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImagePath { get; set; }
        public string? UserToken { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool IsVerified { get; set; } = false;
        public int? CreatedByUserID { get; set; }
        public int? UpdatedByUserID { get; set; }
        public int? DeletedByUserID { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public string? RoleTitle { get; set; }
    }
}
