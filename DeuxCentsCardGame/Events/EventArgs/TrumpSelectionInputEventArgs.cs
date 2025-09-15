using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class TrumpSelectionInputEventArgs : System.EventArgs
    {
        public Player SelectingPlayer { get; }
        public string Response { get; set; } = string.Empty;

        public TrumpSelectionInputEventArgs(Player selectingPlayer)
        {
            SelectingPlayer = selectingPlayer;
        }
    }
}