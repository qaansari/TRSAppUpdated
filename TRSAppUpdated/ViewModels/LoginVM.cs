using System.ComponentModel.DataAnnotations;

namespace TRSAppUpdated.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(11, ErrorMessage = "Please fill out minimum 11 characters")]
        [StringLength(320, ErrorMessage = "Please fill out maximum 320 characters")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public string Password { get; set; }
        public bool IsPersistent { get; set; } = false;
    }
    public class ForgetPasswordVM
    {
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(11, ErrorMessage = "Please fill out minimum 11 characters")]
        [StringLength(320, ErrorMessage = "Please fill out maximum 320 characters")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
    }
    public class ResetPasswordVM
    {
        [Required(ErrorMessage = "Please fill out this field")]
        [MinLength(11, ErrorMessage = "Please fill out minimum 11 characters")]
        [StringLength(320, ErrorMessage = "Please fill out maximum 320 characters")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        public string UserToken { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please fill out this field")]
        public string ConfirmPassword { get; set; }
    }
}
