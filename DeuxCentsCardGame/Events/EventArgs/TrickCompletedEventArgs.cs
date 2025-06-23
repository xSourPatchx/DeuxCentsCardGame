using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class TrickCompletedEventArgs // : EventArgs
    {
        public int TrickNumber { get; }
        public Player WinningPlayer { get; }
        public Card WinningCard { get; }
        public List<(Card card, Player player)> PlayedCards { get; }
        public int TrickPoints { get; }

        public TrickCompletedEventArgs(int trickNumber, Player winningPlayer, Card winningCard, List<(Card card, Player player)> playedCards, int trickPoints)
        {
            WinningPlayer = winningPlayer;
            WinningCard = winningCard;
            PlayedCards = new List<(Card card, Player player)>(playedCards);
            TrickPoints = trickPoints;
            TrickNumber = trickNumber;
        }
    }
}