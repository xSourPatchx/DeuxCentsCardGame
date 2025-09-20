using DeuxCentsCardGame.Config;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces;
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
        private readonly List<Player> _players;
        private readonly int _dealerIndex;
        private readonly GameEventManager _eventManager;

        public BettingManager(List<Player> players, int dealerIndex, GameEventManager eventManager)
        {
            _players = players;
            _dealerIndex = dealerIndex;
            _eventManager = eventManager;
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
                IsBettingRoundComplete = HandleBettingRound(startingIndex);
            }
        }

        private void CompleteBettingRound()
        {
            DetermineWinningBid();

            if (CurrentWinningBid > 0)
            {
                Player winningBidder = _players[CurrentWinningBidIndex];
                var allBids = _players.ToDictionary(player => player, player => player.CurrentBid);
        
                _eventManager.RaiseBettingCompleted(winningBidder, CurrentWinningBid, allBids);
            }
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

        private bool HandleBettingRound(int startingIndex)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                int currentPlayerIndex = (startingIndex + i) % _players.Count;
                Player currentPlayer = _players[currentPlayerIndex];

                // Skip players who have passed
                if (currentPlayer.HasPassed)
                    continue;

                // Parse and handle user input
                if (HandlePlayerBids(currentPlayerIndex))
                    return true; // End betting round

                // Check if three or more players have passed
                if (_players.Count(pass => pass.HasPassed) >= GameConfig.MinimumPlayersToPass)
                {
                    return HandleThreePassesScenario();   
                }
            }

            return false;
        }

        private bool HandlePlayerBids(int currentPlayerIndex)
        {
            while (true)
            {
                string betInput = _eventManager.RaiseBetInput(
                                _players[currentPlayerIndex], 
                                GameConfig.MinimumBet, 
                                GameConfig.MaximumBet, 
                                GameConfig.BetIncrement
                );

                if (betInput == "pass")
                {
                    HandlePassInput(currentPlayerIndex);
                    return false;
                }

                if (int.TryParse(betInput, out int bet) && IsValidBet(bet))
                {
                    return HandleValidBet(currentPlayerIndex, bet);
                }

                _eventManager.RaiseInvalidBet("\nInvalid bet, please try again.\n");
            }
        }

        private void HandlePassInput(int currentPlayerIndex)
        {
            var player = _players[currentPlayerIndex];
            player.HasPassed = true;

            // Only set to -1 if they haven't placed a bet yet
            if (!player.HasBet)
            {
                player.CurrentBid = -1;
            }

            _eventManager.RaiseBettingAction(player, player.CurrentBid, true);
        }

        private bool IsValidBet(int bet)
        {
            return bet >= GameConfig.MinimumBet &&
                   bet <= GameConfig.MaximumBet &&
                   bet % GameConfig.BetIncrement == 0 &&
                   !_players.Any(player => player.CurrentBid == bet);
        }

        private bool HandleValidBet(int currentPlayerIndex, int bet)
        {
            var player = _players[currentPlayerIndex];
            player.CurrentBid = bet;
            player.HasBet = true;

            _eventManager.RaiseBettingAction(player, bet, false);

            if (bet == GameConfig.MaximumBet)
            {
                return HandleMaximumBetScenario(currentPlayerIndex); // End round immediately
            }

            return false;
        }

        private bool HandleMaximumBetScenario(int playerIndex)
        {
            // Force all other players to pass when someone bets 100
            for (int otherPlayerIndex = 0; otherPlayerIndex < _players.Count; otherPlayerIndex++)
            {
                if (otherPlayerIndex != playerIndex && !_players[otherPlayerIndex].HasPassed)
                {
                    var otherPlayer = _players[otherPlayerIndex];
                    otherPlayer.HasPassed = true;

                    // Preserve player existing bid if they had one
                    if (!otherPlayer.HasBet)
                    {
                        otherPlayer.CurrentBid = -1;
                    }

                    _eventManager.RaiseBettingAction(otherPlayer, otherPlayer.CurrentBid, true);
                }
            }

            return true;
        }

        private bool HandleThreePassesScenario()
        {
            var activePlayers = _players.Where(pass => !pass.HasPassed).ToList();
            
            // Check if three players have passed and one remains
            if (activePlayers.Count == 1)
            {
                // Check if this is the "first three pass" scenario or no one has placed a bet yet
                if (!_players.Any(p => p.HasBet && p.CurrentBid > 0))
                {
                    // Force the last player to bet 50
                    activePlayers[0].HasBet = true;
                    activePlayers[0].CurrentBid = GameConfig.MinimumBet;
                    activePlayers[0].HasPassed = true; // Mark as passed to end the round
                    
                    _eventManager.RaiseBettingAction(activePlayers[0], GameConfig.MinimumBet, false);
                }
            }
            
            return true;
        }

        private void DetermineWinningBid()
        {
            // Only consider valid bids (> 0)
            var validBids = _players.Where(player => player.CurrentBid > 0);
            
            if (validBids.Any())
            {
                CurrentWinningBid = validBids.Max(player => player.CurrentBid);
                CurrentWinningBidIndex = _players.FindIndex(player => player.CurrentBid == CurrentWinningBid);
            }
            else
            {
                CurrentWinningBid = 0;
                CurrentWinningBidIndex = -1;
            }
        }
    }
}