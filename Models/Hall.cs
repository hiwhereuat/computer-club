using System;
using System.Collections.Generic;

namespace computerclub.Models;

public partial class Hall
{
    public int HallId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Computer> Computers { get; set; } = new List<Computer>();

    public virtual ICollection<Tariff> Tariffs { get; set; } = new List<Tariff>();
}
