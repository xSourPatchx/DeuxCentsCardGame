using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class DeckCutInputEventArgs : System.EventArgs
    {
        public Player CuttingPlayer { get; }
        public int DeckSize { get; }
        public int Response { get; set; } = -1;

        public DeckCutInputEventArgs(Player cuttingPlayer, int deckSize)
        {
            CuttingPlayer = cuttingPlayer;
            DeckSize = deckSize;
        }
    }
}