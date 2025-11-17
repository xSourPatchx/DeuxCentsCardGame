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

        // Additional properties for AI decision-making
        public List<int> TakenBids { get; set; } = new List<int>();
        public int CurrentHighestBid { get; set; } = 0;
        
        public BetInputEventArgs(Player currentPlayer, int minimumBet, int maximumBet, int betIncrement)
        {
            CurrentPlayer = currentPlayer;
            MinimumBet = minimumBet;
            MaximumBet = maximumBet;
            BetIncrement = betIncrement;
        }
        
        public BetInputEventArgs(Player currentPlayer, int minimumBet, int maximumBet, int betIncrement, 
                                List<int> takenBids, int currentHighestBid)
            : this(currentPlayer, minimumBet, maximumBet, betIncrement)
        {
            TakenBids = takenBids ?? new List<int>();
            CurrentHighestBid = currentHighestBid;
        }
    }
}