using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class TrumpSelectedEventArgs : System.EventArgs
    {
        public CardSuit TrumpSuit { get; }
        public Player SelectedBy { get; }

        public TrumpSelectedEventArgs(CardSuit trumpSuit, Player selectedBy)
        {
            TrumpSuit = trumpSuit;
            SelectedBy = selectedBy;
        }
    }
}