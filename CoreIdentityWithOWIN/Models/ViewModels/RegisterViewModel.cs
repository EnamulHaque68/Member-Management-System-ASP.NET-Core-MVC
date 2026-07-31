using System.ComponentModel.DataAnnotations;

namespace CoreIdentityWithOWIN.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required,EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password),Compare("Password",ErrorMessage ="Password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
