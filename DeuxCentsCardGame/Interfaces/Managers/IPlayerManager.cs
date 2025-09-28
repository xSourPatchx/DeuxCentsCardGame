using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Managers
{
    public interface IPlayerManager
    {
        IReadOnlyList<Player> Players { get; }
        Player GetPlayer(int index);
        void ResetAllPlayerBettingStates();
        void ClearAllPlayerHands();
    }
}
