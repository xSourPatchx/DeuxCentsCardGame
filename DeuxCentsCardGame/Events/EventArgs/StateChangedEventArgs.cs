using DeuxCentsCardGame.GameStates;

namespace DeuxCentsCardGame.Events.EventArgs
{
    // Event arguments for state transition events
    public class StateChangedEventArgs : System.EventArgs
    {
        public GameState PreviousState { get; }
        public GameState NewState { get; }
        
        /// Timestamp when the state change occurred
        public DateTime Timestamp { get; }

        public StateChangedEventArgs(GameState previousState, GameState newState)
        {
            PreviousState = previousState;
            NewState = newState;
            Timestamp = DateTime.UtcNow;
        }
    }
}