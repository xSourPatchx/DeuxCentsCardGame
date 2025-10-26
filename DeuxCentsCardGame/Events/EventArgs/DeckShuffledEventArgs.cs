namespace DeuxCentsCardGame.Events.EventArgs
{
    public class DeckShuffledEventArgs : System.EventArgs
    {
        public string Message { get; }

        public DeckShuffledEventArgs(string message)
        {
            Message = message;
        }
    }
}