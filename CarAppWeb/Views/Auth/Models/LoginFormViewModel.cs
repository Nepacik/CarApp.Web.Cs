using System.ComponentModel.DataAnnotations;

namespace CarAppWeb.Views.Auth.Models
{
    public class LoginFormViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}