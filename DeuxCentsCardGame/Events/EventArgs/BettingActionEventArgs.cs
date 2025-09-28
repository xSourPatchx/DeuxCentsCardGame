using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class BettingActionEventArgs : System.EventArgs
    {
        public Player Player { get; }
        public int Bet { get; }
        public bool HasPassed { get; }
        public bool HasBet { get; }

        public BettingActionEventArgs(Player player, int bet, bool hasPassed = false, bool hasBet = false)
        {
            Player = player;
            Bet = bet;
            HasPassed = hasPassed;
            HasBet = hasBet;
        }
    }
}