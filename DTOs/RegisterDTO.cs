using System.ComponentModel.DataAnnotations;

namespace ParkHub_API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string? NamaLengkap { get; set; }
        [Required]
        [MinLength(4)]

        public string? Username { get; set; }
        [Required]
        [MinLength(8)]

        public string? Password { get; set; }
        [Required]
        public string Role { get; set; } = null!;

    }
}
