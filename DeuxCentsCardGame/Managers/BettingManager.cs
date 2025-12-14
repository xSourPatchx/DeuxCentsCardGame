using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Gameplay;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Validators;

namespace DeuxCentsCardGame.Managers
{
    public class BettingManager : IBettingManager
    {
        // Public betting state properties
        public int CurrentWinningBid { get; set; }
        public int CurrentWinningBidIndex { get; set; }
        public bool IsBettingRoundComplete { get; private set; }

        // Dependencies
        private readonly IGameConfig _gameConfig;
        private readonly List<Player> _players;
        private readonly IGameEventManager _eventManager;
        private readonly GameValidator _gameValidator;
        private readonly BettingLogic _bettingLogic;
        private int _dealerIndex;

        public BettingManager(
            List<Player> players,
            int dealerIndex,
            IGameEventManager eventManager,
            IGameConfig gameConfig,
            GameValidator bettingValidator,
            BettingLogic bettingLogic)
        {
            _players = players ?? throw new ArgumentNullException(nameof(players));
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _dealerIndex = dealerIndex;
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
            _gameValidator = bettingValidator ?? throw new ArgumentNullException(nameof(bettingValidator));
            _bettingLogic = bettingLogic ?? throw new ArgumentNullException(nameof(bettingLogic));
        }

        public async Task ExecuteBettingRound()
        {
            await StartBettingRound();
            await ProcessBettingRound();
            await CompleteBettingRound();
        }

        private async Task StartBettingRound()
        {
            await _eventManager.RaiseBettingRoundStarted("Betting round begins!\n");
        }

        private async Task ProcessBettingRound()
        {
            int startingIndex = (_dealerIndex + 1) % _players.Count;

            while (!IsBettingRoundComplete)
            {
                IsBettingRoundComplete = await ProcessSingleBettingRound(startingIndex);
            }
        }

        private async Task CompleteBettingRound()
        {
            SetWinningBid();
            await RaiseBettingCompletedEvent();
        }

        private async Task<bool> ProcessSingleBettingRound(int startingIndex)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                int currentPlayerIndex = (startingIndex + i) % _players.Count;
                Player currentPlayer = _players[currentPlayerIndex];

                // Skip players who have already passed
                if (_gameValidator.HasPlayerPassed(currentPlayer))
                    continue;

                // Parse and handle user input
                if (await ProcessPlayerBid(currentPlayerIndex))
                    return true;

                // Check if three or more players have passed
                if (_gameValidator.HasMinimumPlayersPassed())
                    return await HandleThreePassesScenario();
            }
            return false;
        }

        private async Task<bool> ProcessPlayerBid(int currentPlayerIndex)
        {
            while (true)
            {
                string betInput = await RequestBetInput(currentPlayerIndex);

                if (_gameValidator.IsPassInput(betInput))
                {
                    await HandlePassInput(currentPlayerIndex);
                    return false;
                }

                if (TryParseBet(betInput, out int bet) && _gameValidator.IsValidBet(bet))
                {
                    return await HandleValidBet(currentPlayerIndex, bet);
                }

                await RaiseInvalidBetEvent(currentPlayerIndex);
            }
        }

        private async Task<string> RequestBetInput(int playerIndex)
        {
            // Get list of already taken bids
            var takenBids = _players
            .Where(p => p.CurrentBid > 0 && !p.HasPassed)
            .Select(p => p.CurrentBid)
            .ToList();

            // Get current highest bid
            int currentHighestBid = takenBids.Any() ? takenBids.Max() : 0;

            // Raise event with enhanced information for AI
            return await _eventManager.RaiseBetInput(
                _players[playerIndex],
                _gameConfig.MinimumBet,
                _gameConfig.MaximumBet,
                _gameConfig.BetIncrement,
                takenBids,
                currentHighestBid
            );
        }

        private bool TryParseBet(string betInput, out int bet)
        {
            return int.TryParse(betInput, out bet);
        }

        private async Task RaiseInvalidBetEvent(int playerIndex)
        {
            await _eventManager.RaiseInvalidMove(
            _players[playerIndex],
            "Invalid bet, please try again.",
            InvalidMoveType.InvalidBet
            );
        }

        private async Task HandlePassInput(int currentPlayerIndex)
        {
            Player player = _players[currentPlayerIndex];
            _bettingLogic.MarkPlayerAsPassed(player);
            await RaiseBettingActionEvent(player, player.CurrentBid, isPassed: true, player.HasBet);
        }

        private async Task<bool> HandleValidBet(int currentPlayerIndex, int bet)
        {
            Player player = _players[currentPlayerIndex];
            _bettingLogic.RecordPlayerBet(player, bet);
            await RaiseBettingActionEvent(player, bet, isPassed: false, hasBet: true);

            if (_gameValidator.IsMaximumBet(bet))
            {
                // End round immediately
                return await HandleMaximumBetScenario(currentPlayerIndex);
            }

            return false;
        }

        private async Task<bool> HandleMaximumBetScenario(int playerIndex)
        {
            _bettingLogic.ForceOtherPlayersToPass(_players, playerIndex);

            // Raise events for forced passes
            for (int i = 0; i < _players.Count; i++)
            {
                if (i != playerIndex && _players[i].HasPassed)
                {
                    await RaiseBettingActionEvent(_players[i], _players[i].CurrentBid, isPassed: true, _players[i].HasBet);
                }
            }   

            return true;
        }

        private async Task<bool> HandleThreePassesScenario()
        {
            List<Player> activePlayers = _bettingLogic.GetActivePlayers(_players);

            if (_bettingLogic.CheckIfOnlyOnePlayerRemains(activePlayers))
            {
                await HandleSingleActivePlayer(activePlayers[0]);
            }

            return true;
        }

        private async Task HandleSingleActivePlayer(Player player)
        {
            if (_bettingLogic.NoBetsPlaced(_players))
            {
                _bettingLogic.ForceMinimumBet(player);
                await RaiseBettingActionEvent(player, _gameConfig.MinimumBet, isPassed: false, hasBet: true);
            }
            else if (!player.HasBet)
            {
                _bettingLogic.ForcePlayerToPass(player);
                await RaiseBettingActionEvent(player, -1, isPassed: true, hasBet: false);
            }
        }

        private void SetWinningBid()
        {
            var (winningBid, winningBidIndex) = _bettingLogic.DetermineWinningBid(_players);
            CurrentWinningBid = winningBid;
            CurrentWinningBidIndex = winningBidIndex;
        }

        private async Task RaiseBettingCompletedEvent()
        {
            if (CurrentWinningBid > 0)
            {
                Player winningBidder = _players[CurrentWinningBidIndex];
                var allBids = _players.ToDictionary(player => player, player => player.CurrentBid);
                await _eventManager.RaiseBettingCompleted(winningBidder, CurrentWinningBid, allBids);
            }
        }

        private async Task RaiseBettingActionEvent(Player player, int bid, bool isPassed, bool hasBet)
        {
            await _eventManager.RaiseBettingAction(player, bid, isPassed, hasBet);
        }

        public async Task ResetBettingRound()
        {
            foreach (var player in _players)
            {
                player.ResetBettingState();
            }

            CurrentWinningBid = 0;
            CurrentWinningBidIndex = 0;
            IsBettingRoundComplete = false;

            await Task.CompletedTask;
        }
        
        public async Task UpdateDealerIndex(int newDealerIndex)
        {
            _dealerIndex = newDealerIndex;
            await Task.CompletedTask;
        }
    }
}