using System;
using System.Collections.Generic;

namespace computerclub.Models;

public partial class ClientRank
{
    public int RankId { get; set; }

    public string RankName { get; set; } = null!;

    public decimal DiscountPercent { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
