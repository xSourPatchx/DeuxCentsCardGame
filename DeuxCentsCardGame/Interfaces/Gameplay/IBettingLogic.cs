using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Gameplay
{
    public interface IBettingLogic
    {
        void MarkPlayerAsPassed(Player player);
        void RecordPlayerBet(Player player, int bet);
        void ForceOtherPlayersToPass(List<Player> players, int maxBetPlayerIndex);
        List<Player> GetActivePlayers(List<Player> players);
        bool CheckIfOnlyOnePlayerRemains(List<Player> activePlayers);
        bool NoBetsPlaced(List<Player> players);
        void ForceMinimumBet(Player player);
        void ForcePlayerToPass(Player player);
        (int winningBid, int winningBidIndex) DetermineWinningBid(List<Player> players);
    }
}