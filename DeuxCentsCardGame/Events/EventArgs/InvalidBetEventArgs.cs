using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Events.EventArgs
{
    public class InvalidBetEventArgs : System.EventArgs
    {
        public string Message { get; }

        public InvalidBetEventArgs(string message)
        {
            Message = message;
        }
    }
}