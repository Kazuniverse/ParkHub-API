using System;
using System.Collections.Generic;

namespace ParkHub_API.Models;

public partial class LogAktivita
{
    public int Id { get; set; }

    public string Aktivitas { get; set; } = null!;

    public DateTime WaktuAktivitas { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
