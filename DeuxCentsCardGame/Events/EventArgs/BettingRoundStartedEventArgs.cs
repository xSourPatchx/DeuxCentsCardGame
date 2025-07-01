namespace DeuxCentsCardGame.Events.EventArgs
{
    public class BettingRoundStartedEventArgs : System.EventArgs
    {
        public string Message { get; }

        public BettingRoundStartedEventArgs(string message)
        {
            Message = message;
        }
    }
}