using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Validators
{
    public class BettingValidator
    {
        private readonly IGameConfig _gameConfig;
        private readonly List<Player> _players;

        public BettingValidator(IGameConfig gameConfig, List<Player> players)
        {
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
            _players = players ?? throw new ArgumentNullException(nameof(players));
        }

        public bool IsValidBet(int bet)
        {
            return IsBetInValidRange(bet) &&
                IsBetValidIncrement(bet) &&
                IsBetUnique(bet);
        }

        public bool IsBetInValidRange(int bet)
        {
            return bet >= _gameConfig.MinimumBet && bet <= _gameConfig.MaximumBet;
        }

        public bool IsBetValidIncrement(int bet)
        {
            return bet % _gameConfig.BetIncrement == 0;
        }

        public bool IsBetUnique(int bet)
        {
            return !_players.Any(player => player.CurrentBid == bet);
        }

        public bool IsMaximumBet(int bet)
        {
            return bet == _gameConfig.MaximumBet;
        }

        public bool HasMinimumPlayersPassed()
        {
            int passedPlayersCount = _players.Count(p => p.HasPassed);
            return passedPlayersCount >= _gameConfig.MinimumPlayersToPass;
        }

        public bool HasPlayerPassed(Player player)
        {
            return player.HasPassed;
        }

        public bool IsPassInput(string input)
        {
            return input == "pass";
        }
    }
}