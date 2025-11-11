using DeuxCentsCardGame.GameStates;

namespace DeuxCentsCardGame.Events.EventArgs
{
    // Event arguments for when the game is paused
    public class GamePausedEventArgs : System.EventArgs
    {
        public GameState StatePausedFrom { get; }
        public DateTime PausedAt { get; }

        public GamePausedEventArgs(GameState statePausedFrom)
        {
            StatePausedFrom = statePausedFrom;
            PausedAt = DateTime.UtcNow;
        }
    }
}