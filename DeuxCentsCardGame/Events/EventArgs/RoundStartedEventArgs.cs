using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class RoundStartedEventArgs : System.EventArgs
    {
        public int RoundNumber { get; }
        public Player Dealer { get; }

        public RoundStartedEventArgs(int roundNumber, Player dealer)
        {
            RoundNumber = roundNumber;
            Dealer = dealer;
        }
    }
}