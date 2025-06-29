using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class RoundEventArgs : System.EventArgs
    {
        public int RoundNumber { get; }
        public Player Dealer { get; }

        public RoundEventArgs(int roundNumber, Player dealer)
        {
            RoundNumber = roundNumber;
            Dealer = dealer;
        }
    }
}