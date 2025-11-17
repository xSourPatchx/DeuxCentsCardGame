using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class BettingManager : IBettingManager
    {
        // Public betting state properties
        public int CurrentWinningBid { get; set; }
        public int CurrentWinningBidIndex { get; set; }
        public bool IsBettingRoundComplete { get; private set; }

        // Private betting state fields
        private readonly IGameConfig _gameConfig;
        private readonly List<Player> _players;
        private readonly IGameEventManager _eventManager;
        private int _dealerIndex;

        public BettingManager(List<Player> players, int dealerIndex, IGameEventManager eventManager, IGameConfig gameConfig)
        {
            _players = players;
            _eventManager = eventManager;
            _dealerIndex = dealerIndex;
            _gameConfig = gameConfig;
        }

        public void ExecuteBettingRound()
        {
            StartBettingRound();
            ProcessBettingRound();
            CompleteBettingRound();
        }

        private void StartBettingRound()
        {
            _eventManager.RaiseBettingRoundStarted("Betting round begins!\n");
        }

        private void ProcessBettingRound()
        {
            int startingIndex = (_dealerIndex + 1) % _players.Count;

            while (!IsBettingRoundComplete)
            {
                IsBettingRoundComplete = ProcessSingleBettingRound(startingIndex);
            }
        }

        private void CompleteBettingRound()
        {
            DetermineWinningBid();
            RaiseBettingCompletedEvent();
        }

        private bool ProcessSingleBettingRound(int startingIndex)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                int currentPlayerIndex = (startingIndex + i) % _players.Count;
                Player currentPlayer = _players[currentPlayerIndex];

                // Skip players who have already passed
                if (SkipPlayer(currentPlayer))
                    continue;

                // Parse and handle user input
                if (ProcessPlayerBid(currentPlayerIndex))
                    return true;

                // Check if three or more players have passed
                if (EndBettingRound())
                    return HandleThreePassesScenario();
            }
            return false;
        }

        private bool SkipPlayer(Player player)
        {
            return player.HasPassed;
        }

        private bool EndBettingRound()
        {
            int passedPlayersCount = _players.Count(pass => pass.HasPassed);
            return passedPlayersCount >= _gameConfig.MinimumPlayersToPass;
        }

        private bool ProcessPlayerBid(int currentPlayerIndex)
        {
            while (true)
            {
                string betInput = RequestBetInput(currentPlayerIndex);

                if (IsPassInput(betInput))
                {
                    HandlePassInput(currentPlayerIndex);
                    return false;
                }

                if (ParseAndValidateBet(betInput, out int bet))
                {
                    return HandleValidBet(currentPlayerIndex, bet);
                }

                RaiseInvalidBetEvent(currentPlayerIndex);
            }
        }

        private string RequestBetInput(int playerIndex)
        {
            // Get list of already taken bids
            var takenBids = _players
            .Where(p => p.CurrentBid > 0)
            .Select(p => p.CurrentBid)
            .ToList();

            // Get current highest bid
            int currentHighestBid = _players
                .Where(p => p.CurrentBid > 0)
                .Select(p => p.CurrentBid)
                .DefaultIfEmpty(0)
                .Max();

            // Raise event with enhanced information for AI
            return _eventManager.RaiseBetInput(
                _players[playerIndex],
                _gameConfig.MinimumBet,
                _gameConfig.MaximumBet,
                _gameConfig.BetIncrement,
                takenBids,
                currentHighestBid
            );
        }

        private bool IsPassInput(string input)
        {
            return input == "pass";
        }

        private bool ParseAndValidateBet(string betInput, out int bet)
        {
            if (!int.TryParse(betInput, out bet))
                return false;

            return IsValidBet(bet);
        }

        private void RaiseInvalidBetEvent(int playerIndex)
        {
            _eventManager.RaiseInvalidMove(
            _players[playerIndex],
            "Invalid bet, please try again.",
            InvalidMoveType.InvalidBet
            );
        }

        private bool IsValidBet(int bet)
        {
            return IsBetInValidRange(bet) &&
                   IsBetValidIncrement(bet) &&
                   IsBetUnique(bet);
        }

        private bool IsBetInValidRange(int bet)
        {
            return bet >= _gameConfig.MinimumBet && bet <= _gameConfig.MaximumBet;
        }

        private bool IsBetValidIncrement(int bet)
        {
            return bet % _gameConfig.BetIncrement == 0;
        }

        private bool IsBetUnique(int bet)
        {
            return !_players.Any(player => player.CurrentBid == bet);
        }

        private void HandlePassInput(int currentPlayerIndex)
        {
            Player player = _players[currentPlayerIndex];

            MarkPlayerAsPassed(player);
            RaiseBettingActionEvent(player, player.CurrentBid, isPassed: true, player.HasBet);
        }

        private void MarkPlayerAsPassed(Player player)
        {
            player.HasPassed = true;

            if (!player.HasBet)
            {
                player.CurrentBid = -1;
            }
        }

        private bool HandleValidBet(int currentPlayerIndex, int bet)
        {
            Player player = _players[currentPlayerIndex];

            RecordPlayerBet(player, bet);
            RaiseBettingActionEvent(player, bet, isPassed: false, hasBet: true);

            if (IsMaximumBet(bet))
            {
                // End round immediately
                return HandleMaximumBetScenario(currentPlayerIndex);
            }

            return false;
        }

        private void RecordPlayerBet(Player player, int bet)
        {
            player.CurrentBid = bet;
            player.HasBet = true;
        }

        private bool IsMaximumBet(int bet)
        {
            return bet == _gameConfig.MaximumBet;
        }

        private bool HandleMaximumBetScenario(int playerIndex)
        {
            ForceOtherPlayersToPass(playerIndex);
            return true;
        }

        private void ForceOtherPlayersToPass(int maxBetPlayerIndex)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (i == maxBetPlayerIndex || _players[i].HasPassed)
                    continue;
                
                Player otherPlayer = _players[i];
                MarkPlayerAsPassed(otherPlayer);
                RaiseBettingActionEvent(otherPlayer, otherPlayer.CurrentBid, isPassed: true, otherPlayer.HasBet);
            }
        }

        private bool HandleThreePassesScenario()
        {
            List<Player> activePlayers = GetActivePlayers();

            if (CheckIfOnlyOnePlayerRemains(activePlayers))
            {
                HandleSingleActivePlayer(activePlayers[0]);
            }

            return true;
        }

        private List<Player> GetActivePlayers()
        {
            return _players.Where(player => !player.HasPassed).ToList();
        }

        private bool CheckIfOnlyOnePlayerRemains(List<Player> activePlayers)
        {
            return activePlayers.Count == 1;
        }

        private void HandleSingleActivePlayer(Player player)
        {
            if (NoBetsPlaced())
            {
                ForceMinimumBet(player);
            }
            else if (!player.HasBet)
            {
                ForcePlayerToPass(player);
            }
        }

        private bool NoBetsPlaced()
        {
            return !_players.Any(p => p.HasBet && p.CurrentBid > 0);
        }

        private void ForceMinimumBet(Player player)
        {
            player.HasBet = true;
            player.CurrentBid = _gameConfig.MinimumBet;
            RaiseBettingActionEvent(player, _gameConfig.MinimumBet, isPassed: false, hasBet: true);
        }
        
        private void ForcePlayerToPass(Player player)
        {
            player.HasPassed = true;
            player.CurrentBid = -1;
            RaiseBettingActionEvent(player, -1, isPassed: true, hasBet: false);    
        }

        private void DetermineWinningBid()
        {
            var validBids = GetValidBids();

            if (validBids.Any())
            {
                SetWinningBid(validBids);
            }
            else
            {          
                ClearWinningBid();
            }
        }

        private IEnumerable<Player> GetValidBids()
        {
            return _players.Where(player => player.CurrentBid > 0);
        }

        private void SetWinningBid(IEnumerable<Player> validBids)
        {
            CurrentWinningBid = validBids.Max(player => player.CurrentBid);
            CurrentWinningBidIndex = _players.FindIndex(player => player.CurrentBid == CurrentWinningBid);
        }
        
        private void ClearWinningBid()
        {
            CurrentWinningBid = 0;
            CurrentWinningBidIndex = -1;
        }

        private void RaiseBettingCompletedEvent()
        {
            if (CurrentWinningBid > 0)
            {
                Player winningBidder = _players[CurrentWinningBidIndex];
                var allBids = _players.ToDictionary(player => player, player => player.CurrentBid);

                _eventManager.RaiseBettingCompleted(winningBidder, CurrentWinningBid, allBids);
            }
        }

        private void RaiseBettingActionEvent(Player player, int bid, bool isPassed, bool hasBet)
        {
            _eventManager.RaiseBettingAction(player, bid, isPassed, hasBet);
        }

        public void ResetBettingRound()
        {
            foreach (var player in _players)
            {
                player.ResetBettingState();
            }

            CurrentWinningBid = 0;
            CurrentWinningBidIndex = 0;
            IsBettingRoundComplete = false;
        }
        
        public void UpdateDealerIndex(int newDealerIndex)
        {
            _dealerIndex = newDealerIndex;
        }
    }
}