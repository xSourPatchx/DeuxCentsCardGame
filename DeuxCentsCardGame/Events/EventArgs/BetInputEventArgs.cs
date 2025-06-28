using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class BetInputEventArgs : System.EventArgs
    {
        public Player CurrentPlayer { get; }
        public int MinimumBet { get; }
        public int MaximumBet { get; }
        public int BetIncrement { get; }
        public string Response { get; set; } = string.Empty;
        
        public BetInputEventArgs(Player currentPlayer, int minimumBet, int maximumBet, int betIncrement)
        {
            CurrentPlayer = currentPlayer;
            MinimumBet = minimumBet;
            MaximumBet = maximumBet;
            BetIncrement = betIncrement;
        }
    }
}