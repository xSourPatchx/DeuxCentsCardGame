using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class PlayerTurnEventArgs : System.EventArgs
    {
        public Player Player { get; }
        public CardSuit? LeadingSuit { get; }
        public CardSuit? TrumpSuit { get; }
        public int TrickNumber { get; }

        public PlayerTurnEventArgs(Player player, CardSuit? leadingSuit, CardSuit? trumpSuit, int trickNumber)
        {
            Player = player;
            LeadingSuit = leadingSuit;
            TrumpSuit = trumpSuit;
            TrickNumber = trickNumber;
        }
    }
}