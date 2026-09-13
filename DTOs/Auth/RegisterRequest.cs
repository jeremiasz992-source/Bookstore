using System.ComponentModel.DataAnnotations;

namespace Bookstore.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; } = "";
    }
}