using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class NextRoundEventArgs : System.EventArgs
    {
        public string Message { get; }

        public NextRoundEventArgs(string message = "Press any key to start the next round...")
        {
            Message = message;
        }
    }
}