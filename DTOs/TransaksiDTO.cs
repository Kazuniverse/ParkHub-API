using System.ComponentModel.DataAnnotations;

namespace ParkHub_API.DTOs
{
    public class TransaksiDTO
    {
        [Required]
        public string PlatNomor { get; set; } = null!;
        [Required]

        public string NamaArea { get; set; } = null!;
    }
}
