namespace DeuxCentsCardGame.GameStates
{
    public class GameStateData
    {
        public GameState CurrentState { get; set; }
        public GameState PreviousState { get; set; }
        public int CurrentRound { get; set; }
        public int CurrentTrick { get; set; }

        // For pause/resume functionality
        public bool IsPaused { get; set; }
        public GameState StateBeforePause { get; set; }
    }
}