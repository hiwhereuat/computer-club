using System;
using System.Collections.Generic;

namespace computerclub.Models
{
    public partial class Game
    {
        public int GameId { get; set; }
        public string Title { get; set; } = null!;
        public string? Developer { get; set; }

        public virtual ICollection<ComputerGame> ComputerGames { get; set; } = new HashSet<ComputerGame>();
    }
}