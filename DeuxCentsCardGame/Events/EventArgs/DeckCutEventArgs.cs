using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class DeckCutEventArgs : System.EventArgs
    {
        public Player CuttingPlayer { get; }
        public int CutPosition { get; }

        public DeckCutEventArgs(Player cuttingPlayer, int cutPosition)
        {
            CuttingPlayer = cuttingPlayer;
            CutPosition = cutPosition;
        }
    }
}