using System;
using System.Collections.Generic;

namespace computerclub.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public int PositionId { get; set; }

    public string? Login { get; set; }

    public string? PasswordHash { get; set; }

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Position Position { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
