using System.ComponentModel.DataAnnotations;

namespace CarAppWeb.Dtos
{
    public class ModelDto
    {
        [Required]
        public string Name { get; set; }
    }
}