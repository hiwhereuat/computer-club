using System;
using System.Collections.Generic;

namespace computerclub.Models;

public partial class Client
{
    public int ClientId { get; set; }

    public string Nickname { get; set; } = null!;

    public string? Phone { get; set; }

    public decimal Balance { get; set; }

    public int RankId { get; set; }

    public virtual ClientRank Rank { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
