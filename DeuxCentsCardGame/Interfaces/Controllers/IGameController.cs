using DeuxCentsCardGame.GameStates;

namespace DeuxCentsCardGame.Interfaces.Controllers
{
    public interface IGameController
    {
        Task TransitionToState(GameState newState);
        GameState GetCurrentState();
        int GetCurrentRound();
        int GetCurrentTrick();
        Task PauseGame();
        Task ResumeGame();
        bool IsPaused();
        Task StartGame();
        Task NewRound();
    }
}