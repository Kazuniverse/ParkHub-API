using System;
using System.Collections.Generic;

namespace ParkHub_API.Models;

public partial class Transaksi
{
    public int Id { get; set; }

    public DateTime WaktuMasuk { get; set; }

    public DateTime? WaktuKeluar { get; set; }

    public int Durasi { get; set; }

    public decimal BiayaTotal { get; set; }

    public string Status { get; set; } = null!;

    public int? UserId { get; set; }

    public int? AreaId { get; set; }

    public int? KendaraanId { get; set; }

    public int? TarifId { get; set; }

    public virtual AreaParkir? Area { get; set; }

    public virtual Kendaraan? Kendaraan { get; set; }

    public virtual Tarif? Tarif { get; set; }

    public virtual User? User { get; set; }
}
