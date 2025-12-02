using DeuxCentsCardGame.GameStates;

namespace DeuxCentsCardGame.Interfaces.GameStates
{
    public interface IGameStateData
    {
        GameState CurrentState { get; set; }
        GameState PreviousState { get; set; }
        int CurrentRound { get; set; }
        int CurrentTrick { get; set; }
        bool IsPaused { get; set; }
        GameState StateBeforePause { get; set; }
    }
}