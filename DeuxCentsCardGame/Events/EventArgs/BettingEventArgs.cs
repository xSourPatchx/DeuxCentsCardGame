using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class BettingEventArgs : System.EventArgs
    {
        public Player Player { get; }
        public int Bet { get; }
        public bool HasPassed { get; }

        public BettingEventArgs(Player player, int bet, bool hasPassed = false)
        {
            Player = player;
            Bet = bet;
            HasPassed = hasPassed;
        }
    }
}