using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class CardPlayedEventArgs : System.EventArgs
    {
        public Player Player { get; }
        public Card Card { get; }
        public int TrickNumber { get; }
        public CardSuit? LeadingSuit { get; }
        public CardSuit? TrumpSuit { get; }

        public CardPlayedEventArgs(Player player, Card card, int trickNumber, CardSuit? leadingSuit, CardSuit? trumpSuit)
        {
            TrickNumber = trickNumber;
            Player = player;
            Card = card;
            LeadingSuit = leadingSuit;
            TrumpSuit = trumpSuit;
        }
    }
}