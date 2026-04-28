using System;
using System.Collections.Generic;

namespace computerclub.Models;

public partial class Tariff
{
    public int TariffId { get; set; }

    public int HallId { get; set; }

    public string TariffName { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public decimal Price { get; set; }

    public virtual Hall Hall { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
