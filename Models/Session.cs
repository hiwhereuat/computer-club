using System;
using System.Collections.Generic;

namespace computerclub.Models;

public partial class Session
{
    public int SessionId { get; set; }

    public int ClientId { get; set; }

    public int ComputerId { get; set; }

    public int EmployeeId { get; set; }

    public int TariffId { get; set; }

    public int PaymentMethodId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal TotalCost { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Computer Computer { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual Tariff Tariff { get; set; } = null!;
}
