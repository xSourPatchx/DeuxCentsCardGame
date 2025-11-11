using DeuxCentsCardGame.GameStates;

namespace DeuxCentsCardGame.Interfaces.Controllers
{
    public interface IGameController
    {
        void StartGame();
        void NewRound();
        void ExecuteBettingRound();

        // State management methods
        GameState GetCurrentState();
        int GetCurrentRound();
        int GetCurrentTrick();
        void PauseGame();
        void ResumeGame();
        bool IsPaused();
    }
}