using System;
using System.Collections.Generic;

namespace computerclub.Models
{
    public partial class Computer
    {
        public int ComputerId { get; set; }
        public int HallId { get; set; }
        public string ComputerName { get; set; } = null!;
        public string Ipaddress { get; set; } = null!;
        public bool IsAvailable { get; set; }

        public virtual Hall Hall { get; set; } = null!;
        public virtual ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
        public virtual ICollection<ComputerGame> ComputerGames { get; set; } = new HashSet<ComputerGame>();
    }
}