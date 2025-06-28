using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class TrumpSelectionInputEventArgs : System.EventArgs
    {
        public Player SelectingPlayer { get; }
        
        public TrumpSelectionInputEventArgs(Player selectingPlayer)
        {
            SelectingPlayer = selectingPlayer;
        }
    }
}