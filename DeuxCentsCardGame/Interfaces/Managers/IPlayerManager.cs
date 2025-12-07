using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Managers
{
    public interface IPlayerManager
    {
        IReadOnlyList<Player> Players { get; }
        Player GetPlayer(int index);
        Task ResetAllPlayerBettingStates();
        Task ClearAllPlayerHands();
    }
}
