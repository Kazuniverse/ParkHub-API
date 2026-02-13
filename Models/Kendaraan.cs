using System;
using System.Collections.Generic;

namespace ParkHub_API.Models;

public partial class Kendaraan
{
    public int Id { get; set; }

    public string PlatNomor { get; set; } = null!;

    public string Warna { get; set; } = null!;

    public string Pemilik { get; set; } = null!;

    public int JenisKendaraanId { get; set; }

    public string? ImagePath { get; set; }

    public virtual JenisKendaraan JenisKendaraan { get; set; } = null!;

    public virtual ICollection<Transaksi> Transaksis { get; set; } = new List<Transaksi>();
}
