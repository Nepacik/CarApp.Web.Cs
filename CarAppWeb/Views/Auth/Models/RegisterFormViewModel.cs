using System.ComponentModel.DataAnnotations;

namespace CarAppWeb.Views.Auth.Models
{
    public class RegisterFormViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required(ErrorMessage = "RequiredField")]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password doesn't match")]
        public string ConfirmPassword { get; set; }
    }
}