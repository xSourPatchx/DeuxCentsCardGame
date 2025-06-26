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

        public List<int> PlayerBids { get; set; }
        public List<bool> PlayerHasBet { get; set; }
        public List<bool> PlayerHasPassed { get; set; }

        // Private betting state fields
        private readonly List<Player> _players;
        private readonly IUIGameView _ui;
        private readonly int _dealerIndex;
        private readonly GameEventManager _eventManager;

        public BettingState(List<Player> players, IUIGameView ui, int dealerIndex, GameEventManager eventManager)
        {
            _players = players;
            _ui = ui;
            _dealerIndex = dealerIndex;
            _eventManager = eventManager;
        }

        public void ExecuteBettingRound()
        {
            // _ui.DisplayMessage("Betting round begins!\n");

            int startingIndex = (_dealerIndex + 1) % _players.Count;

            while (!IsBettingRoundComplete)
            {
                IsBettingRoundComplete = HandleBettingRound(startingIndex);
            }

            // DisplayBettingResults();
            DetermineWinningBid();

            var allBids = new Dictionary<Player, int>();
            for (int i = 0; i < _players.Count; i++)
            {
                allBids[_players[i]] = PlayerBids[i];
            }

            Player winningBidder = _players[CurrentWinningBidIndex];
            _eventManager.RaiseBettingCompleted(winningBidder, CurrentWinningBid, allBids);

            IsBettingRoundComplete = true;
        }

        public void ResetBettingRound()
        {
            PlayerBids = Enumerable.Repeat(0, _players.Count).ToList();
            PlayerHasBet = Enumerable.Repeat(false, _players.Count).ToList();
            PlayerHasPassed = Enumerable.Repeat(false, _players.Count).ToList();
            CurrentWinningBid = 0;
            CurrentWinningBidIndex = 0;
            IsBettingRoundComplete = false;
        }

        private bool HandleBettingRound(int startingIndex)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                int currentPlayerIndex = (startingIndex + i) % _players.Count;

                if (PlayerHasPassed[currentPlayerIndex])
                    continue;

                if (HandlePlayerBids(currentPlayerIndex))
                    return true;

                if (PlayerHasPassed.Count(pass => pass) >= 3)
                    return HandleThreePassesScenario(currentPlayerIndex);
            }

            return false;
        }

        private bool HandlePlayerBids(int currentPlayerIndex)
        {
            while (true)
            {
                string prompt = $"{_players[currentPlayerIndex].Name}, enter a bet (between {MinimumBet}-{MaximumBet}, intervals of {BetIncrement}) or 'pass': ";
                string betInput = _ui.GetUserInput(prompt).ToLower();

                if (betInput == "pass")
                {
                    HandlePassInput(currentPlayerIndex);
                    return false;
                }

                if (int.TryParse(betInput, out int bet) && IsValidBet(bet))
                {
                    return HandleValidBet(currentPlayerIndex, bet);
                }

                _ui.DisplayMessage("Invalid bet, please try again");
            }
        }

        private void HandlePassInput(int currentPlayerIndex)
        {
            // _ui.DisplayFormattedMessage("\n{0} passed\n", _players[currentPlayerIndex].Name);
            PlayerHasPassed[currentPlayerIndex] = true;
            PlayerBids[currentPlayerIndex] = -1;

            _eventManager.RaiseBettingAction(_players[currentPlayerIndex], -1, true);
        }

        private bool IsValidBet(int bet)
        {
            return bet >= MinimumBet &&
                   bet <= MaximumBet &&
                   bet % BetIncrement == 0 &&
                   !PlayerBids.Contains(bet);
        }

        private bool HandleValidBet(int currentPlayerIndex, int bet)
        {
            PlayerBids[currentPlayerIndex] = bet;
            PlayerHasBet[currentPlayerIndex] = true;

            _eventManager.RaiseBettingAction(_players[currentPlayerIndex], bet, false);

            if (bet == MaximumBet)
            {
                return HandleMaximumBetScenario(currentPlayerIndex); // End the betting round immediately
            }

            return false;
        }

        private bool HandleMaximumBetScenario(int playerIndex)
        {
            // _ui.DisplayFormattedMessage("{0} bet {1}. Betting round ends.\n", _players[playerIndex].Name, MaximumBet);
            for (int otherPlayerIndex = 0; otherPlayerIndex < _players.Count; otherPlayerIndex++)
            {
                if (otherPlayerIndex != playerIndex && !PlayerHasPassed[otherPlayerIndex])
                {
                    PlayerHasPassed[otherPlayerIndex] = true;
                    PlayerBids[otherPlayerIndex] = -1;

                    _eventManager.RaiseBettingAction(_players[otherPlayerIndex], -1, true);
                }
            }

            return true;
        }

        private bool HandleThreePassesScenario(int currentPlayerIndex)
        {
            // _ui.DisplayMessage("Three players have passed, betting round ends");

            // Check if all players passed and no bets placed
            if (PlayerBids.All(bet => bet <= 0))
            {
                int lastBiddingPlayerIndex = (currentPlayerIndex + 1) % _players.Count;
                PlayerHasBet[lastBiddingPlayerIndex] = true;
                PlayerBids[lastBiddingPlayerIndex] = MinimumBet;
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
            CurrentWinningBid = PlayerBids.Max();
            CurrentWinningBidIndex = PlayerBids.IndexOf(CurrentWinningBid);

            // _ui.DisplayFormattedMessage("\n{0} won the bid.", _players[CurrentWinningBidIndex].Name);
            // _ui.DisplayMessage("\n#########################\n");
            // UIGameView.DisplayHand(_players[CurrentWinningBidIndex]);
        }
    }
}