using System;
using System.Collections.Generic;

namespace ParkHub_API.Models;

public partial class User
{
    public int Id { get; set; }

    public string? NamaLengkap { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<LogAktivita> LogAktivita { get; set; } = new List<LogAktivita>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Transaksi> Transaksis { get; set; } = new List<Transaksi>();
}
