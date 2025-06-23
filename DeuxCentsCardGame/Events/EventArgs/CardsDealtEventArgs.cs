using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class CardsDealtEventArgs // : EventArgs
    {
        public List<Player> Players { get; }
        public int DealerIndex { get; }

        public CardsDealtEventArgs(List<Player> players, int dealerIndex)
        {
            Players = new List<Player>(players);
            DealerIndex = dealerIndex;
        }
    }
}