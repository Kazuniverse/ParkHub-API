using System;
using System.Collections.Generic;

namespace ParkHub_API.Models;

public partial class AreaParkir
{
    public int Id { get; set; }

    public string NamaArea { get; set; } = null!;

    public int Kapasitas { get; set; }

    public virtual ICollection<Transaksi> Transaksis { get; set; } = new List<Transaksi>();
}
