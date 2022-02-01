using System.ComponentModel.DataAnnotations;

namespace CarAppWeb.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}