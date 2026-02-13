namespace ParkHub_API.DTOs
{
    public class KendaraanDTO
    {
        public string PlatNomor { get; set; }

        public string Warna { get; set; } = null!;

        public string Pemilik { get; set; } = null!;

        public string JenisKendaraan { get; set; }
        public string? ImagePath { get; set; }
    }
}
