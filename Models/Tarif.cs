using System;
using System.Collections.Generic;

namespace ParkHub_API.Models;

public partial class Tarif
{
    public int Id { get; set; }

    public decimal? TarifPerJam { get; set; }

    public int JenisKendaraanId { get; set; }

    public virtual JenisKendaraan JenisKendaraan { get; set; } = null!;

    public virtual ICollection<Transaksi> Transaksis { get; set; } = new List<Transaksi>();
}
