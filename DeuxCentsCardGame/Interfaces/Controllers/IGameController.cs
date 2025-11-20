using DeuxCentsCardGame.GameStates;

namespace DeuxCentsCardGame.Interfaces.Controllers
{
    public interface IGameController
    {
        void TransitionToState(GameState newState);
        GameState GetCurrentState();
        int GetCurrentRound();
        int GetCurrentTrick();
        void PauseGame();
        void ResumeGame();
        bool IsPaused();
        void StartGame();
        void NewRound();
    }
}