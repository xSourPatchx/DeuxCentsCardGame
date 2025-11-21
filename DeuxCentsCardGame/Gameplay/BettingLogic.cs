using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Gameplay
{
    public class BettingLogic
    {
        private readonly IGameConfig _gameConfig;

        public BettingLogic(IGameConfig gameConfig)
        {
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
        }

        public void MarkPlayerAsPassed(Player player)
        {
            player.HasPassed = true;

            if (!player.HasBet)
            {
                player.CurrentBid = -1;
            }
        }

        public void RecordPlayerBet(Player player, int bet)
        {
            player.CurrentBid = bet;
            player.HasBet = true;
        }

        public void ForceOtherPlayersToPass(List<Player> players, int maxBetPlayerIndex)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (i == maxBetPlayerIndex || players[i].HasPassed)
                    continue;

                MarkPlayerAsPassed(players[i]);
            }
        }

        public List<Player> GetActivePlayers(List<Player> players)
        {
            return players.Where(player => !player.HasPassed).ToList();
        }

        public bool CheckIfOnlyOnePlayerRemains(List<Player> activePlayers)
        {
            return activePlayers.Count == 1;
        }

        public bool NoBetsPlaced(List<Player> players)
        {
            return !players.Any(p => p.HasBet && p.CurrentBid > 0);
        }

        public void ForceMinimumBet(Player player)
        {
            player.HasBet = true;
            player.CurrentBid = _gameConfig.MinimumBet;
        }

        public void ForcePlayerToPass(Player player)
        {
            player.HasPassed = true;
            player.CurrentBid = -1;
        }

        public (int winningBid, int winningBidIndex) DetermineWinningBid(List<Player> players)
        {
            var validBids = GetValidBids(players);

            if (validBids.Any())
            {
                int winningBid = validBids.Max(player => player.CurrentBid);
                int winningBidIndex = players.FindIndex(player => player.CurrentBid == winningBid);
                return (winningBid, winningBidIndex);
            }

            return (0, -1);
        }

        private IEnumerable<Player> GetValidBids(List<Player> players)
        {
            return players.Where(player => player.CurrentBid > 0);
        }
    }
}