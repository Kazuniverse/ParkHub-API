using System;
using System.Collections.Generic;

namespace ParkHub_API.Models;

public partial class JenisKendaraan
{
    public int Id { get; set; }

    public string Jenis { get; set; } = null!;

    public virtual ICollection<Kendaraan> Kendaraans { get; set; } = new List<Kendaraan>();

    public virtual ICollection<Tarif> Tarifs { get; set; } = new List<Tarif>();
}
