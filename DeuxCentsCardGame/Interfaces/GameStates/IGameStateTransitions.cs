using DeuxCentsCardGame.GameStates;

namespace DeuxCentsCardGame.Interfaces.GameStates
{
    /// Manages valid state transitions for the game state machine.
    public interface IGameStateTransitions
    {
        bool IsValidTransition(GameState from, GameState to);
        List<GameState> GetValidNextStates(GameState current);
        void ValidateTransition(GameState from, GameState to);
        bool IsTerminalState(GameState state);
        List<GameState> GetAllStates();
        bool IsBranchingState(GameState state);
    }
}