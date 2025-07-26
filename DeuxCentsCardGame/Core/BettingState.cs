using DeuxCentsCardGame.Interfaces;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.UI;
using DeuxCentsCardGame.Events;

namespace DeuxCentsCardGame.Core
{
    public class BettingState
    {
        // Betting constants (Will move to GameConfig Class)
        public const int MinimumBet = 50;
        public const int MaximumBet = 100;
        public const int BetIncrement = 5;

        // Public betting state properties
        public int CurrentWinningBid { get; set; }
        public int CurrentWinningBidIndex { get; set; }
        public bool IsBettingRoundComplete { get; private set; }

        // Private betting state fields
        private readonly List<Player> _players;
        private readonly int _dealerIndex;
        private readonly GameEventManager _eventManager;

        public BettingState(List<Player> players, int dealerIndex, GameEventManager eventManager)
        {
            _players = players;
            _dealerIndex = dealerIndex;
            _eventManager = eventManager;
        }

        public void ExecuteBettingRound()
        {
            int startingIndex = (_dealerIndex + 1) % _players.Count;

            _eventManager.RaiseBettingRoundStarted("Betting round begins!\n");

            _eventManager.RaiseBettingRoundStarted("Betting round begins!\n");

            while (!IsBettingRoundComplete)
            {
                IsBettingRoundComplete = HandleBettingRound(startingIndex);
            }

            DetermineWinningBid();

            var allBids = new Dictionary<Player, int>();
            Player winningBidder = _players[CurrentWinningBidIndex];

            _eventManager.RaiseBettingCompleted(winningBidder, CurrentWinningBid, allBids);

            IsBettingRoundComplete = true;
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

                if (currentPlayer.HasPassed)
                    continue;

                if (HandlePlayerBids(currentPlayerIndex))
                    return true;

                if (_players.Count(pass => pass.HasPassed) >= 3)
                    return HandleThreePassesScenario(currentPlayerIndex);
            }

            return false;
        }

        private bool HandlePlayerBids(int currentPlayerIndex)
        {
            while (true)
            {
                string betInput = _eventManager.RaiseBetInput(
                                _players[currentPlayerIndex], 
                                MinimumBet, 
                                MaximumBet, 
                                BetIncrement
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
                _eventManager.RaiseInvalidBet("\nInvalid bet, please try again.\n");
            }
        }

        private void HandlePassInput(int currentPlayerIndex)
        {
            var player = _players[currentPlayerIndex];
            // bool hasBet = player.HasBet;
            
            player.HasPassed = true;
            player.CurrentBid = -1;

            _eventManager.RaiseBettingAction(player, -1, true);
        }

        private bool IsValidBet(int bet)
        {
            return bet >= MinimumBet &&
                   bet <= MaximumBet &&
                   bet % BetIncrement == 0 &&
                   !_players.Any(player => player.CurrentBid == bet);
        }

        private bool HandleValidBet(int currentPlayerIndex, int bet)
        {
            var player = _players[currentPlayerIndex];
            player.CurrentBid = bet;
            player.HasBet = true;

            _eventManager.RaiseBettingAction(player, bet, false);

            if (bet == MaximumBet)
            {
                return HandleMaximumBetScenario(currentPlayerIndex); // End the betting round immediately
            }

            return false;
        }

        private bool HandleMaximumBetScenario(int playerIndex)
        {
            for (int otherPlayerIndex = 0; otherPlayerIndex < _players.Count; otherPlayerIndex++)
            {
                if (otherPlayerIndex != playerIndex && !_players[otherPlayerIndex].HasPassed)
                {
                    _players[otherPlayerIndex].HasPassed = true;

                    _eventManager.RaiseBettingAction(_players[otherPlayerIndex], -1, true);
                }
            }

            return true;
        }

        private bool HandleThreePassesScenario(int currentPlayerIndex)
        {
            // Check if all players has placed a valid bet (not -1 and not 0)
            if (_players.Any(pass => pass.CurrentBid > 0))
            {
                int lastBiddingPlayerIndex = (currentPlayerIndex + 1) % _players.Count;
                var lastPlayer = _players[lastBiddingPlayerIndex];

                lastPlayer.HasBet = true;
                lastPlayer.CurrentBid = MinimumBet;

                CurrentWinningBid = MinimumBet;
                CurrentWinningBidIndex = lastBiddingPlayerIndex;

                _eventManager.RaiseBettingAction(_players[lastBiddingPlayerIndex], MinimumBet, false);
            }

            return true;
        }

        // private void DisplayBettingResults()
        // {
        //     _ui.DisplayMessage("\nBetting round complete, here are the results:");

        //     for (int i = 0; i < _players.Count; i++)
        //     {
        //         string result = GetPlayerBettingResult(i);
        //         _ui.DisplayFormattedMessage("{0} : {1}", _players[i].Name, result);
        //     }
        // }

        // private string GetPlayerBettingResult(int playerIndex)
        // {
        //     if (PlayerHasPassed[playerIndex])
        //     {
        //         return PlayerHasBet[playerIndex] ? "Passed after betting" : "Passed";
        //     }
        //     return $"Bet {PlayerBids[playerIndex]}";
        // }

        private void DetermineWinningBid()
        {
            CurrentWinningBid = _players.Max(player => player.CurrentBid);
            CurrentWinningBidIndex = _players.FindIndex(player => player.CurrentBid == CurrentWinningBid);
        }
    }
}