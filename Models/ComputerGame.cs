namespace computerclub.Models
{
    public partial class ComputerGame
    {
        public int ComputerId { get; set; }
        public int GameId { get; set; }

        public virtual Computer Computer { get; set; } = null!;
        public virtual Game Game { get; set; } = null!;
    }
}