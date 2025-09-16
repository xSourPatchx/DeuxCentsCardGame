using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class CardSelectionInputEventArgs : System.EventArgs
    {
        public Player CurrentPlayer { get; }
        public CardSuit? LeadingSuit { get; }
        public CardSuit? TrumpSuit { get; }
        public List<Card> Hand { get; }
        public int Response { get; set; } = -1;

        public CardSelectionInputEventArgs(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit, List<Card> hand)
        {
            CurrentPlayer = currentPlayer;
            LeadingSuit = leadingSuit;
            TrumpSuit = trumpSuit;
            Hand = new List<Card>(hand);
        }
    }
    }