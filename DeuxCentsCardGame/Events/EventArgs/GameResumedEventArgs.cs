using DeuxCentsCardGame.GameStates;

namespace DeuxCentsCardGame.Events.EventArgs
{
    // Event arguments for when the game is resumed
    public class GameResumedEventArgs : System.EventArgs
    {
        public GameState StateResumedTo { get; }
        public DateTime ResumedAt { get; }

        public GameResumedEventArgs(GameState stateResumedTo)
        {
            StateResumedTo = stateResumedTo;
            ResumedAt = DateTime.UtcNow;
        }
    }
}