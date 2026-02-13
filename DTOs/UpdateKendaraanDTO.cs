namespace ParkHub_API.DTOs
{
    public class UpdateKendaraanDTO
    {
        public string Warna { get; set; } = null!;

        public string Pemilik { get; set; } = null!;

        public string JenisKendaraan { get; set; }
        public string? ImagePath { get; set; }
    }
}
