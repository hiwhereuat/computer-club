using System;
using System.Collections.Generic;

namespace computerclub.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int SessionId { get; set; }

    public int EmployeeId { get; set; }

    public int PaymentMethodId { get; set; }

    public DateTime OrderTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual Session Session { get; set; } = null!;
}
