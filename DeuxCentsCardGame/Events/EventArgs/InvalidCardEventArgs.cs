using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class InvalidCardEventArgs : System.EventArgs
    {
        public string Message { get; }

        public InvalidCardEventArgs(string message)
        {
            Message = message;
        }
    }
}