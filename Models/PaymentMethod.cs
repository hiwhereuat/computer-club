using System;
using System.Collections.Generic;

namespace computerclub.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string MethodName { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
