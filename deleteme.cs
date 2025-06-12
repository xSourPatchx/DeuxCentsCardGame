// Deepseek

namespace DeuxCentsCardGame
{
    public class BettingState
    {
        // Betting constants
        public const int MinimumBet = 50;
        public const int MaximumBet = 100;
        public const int BetIncrement = 5;

        // Betting state
        public List<int> Bets { get; private set; }
        public bool[] HasPassed { get; private set; }
        public bool[] PlayerHasBet { get; private set; }
        public int CurrentWinningBid { get; private set; }
        public int CurrentWinningBidIndex { get; private set; }
        public bool IsComplete { get; private set; }

        private readonly List<Player> _players;
        private readonly IUIConsoleGameView _ui;
        private readonly int _dealerIndex;

        public BettingState(List<Player> players, IUIConsoleGameView ui, int dealerIndex)
        {
            _players = players;
            _ui = ui;
            _dealerIndex = dealerIndex;
            
            Bets = new List<int>(new int[_players.Count]);
            HasPassed = new bool[_players.Count];
            PlayerHasBet = new bool[_players.Count];
            CurrentWinningBid = 0;
            CurrentWinningBidIndex = 0;
            IsComplete = false;
        }

        public void ExecuteBettingRound()
        {
            _ui.DisplayMessage("Betting round begins!\n");
            
            int startingIndex = (_dealerIndex + 1) % _players.Count; // Player after dealer starts
            bool bettingRoundEnded = false;

            while (!bettingRoundEnded)
            {
                bettingRoundEnded = HandleBettingRound(startingIndex);
            }

            DisplayBettingResults();
            DetermineWinningBid();
            IsComplete = true;
        }

        private bool HandleBettingRound(int startingIndex)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                int playerIndex = (startingIndex + i) % _players.Count;

                if (HasPassed[playerIndex])
                    continue;

                if (HandlePlayerBets(playerIndex))
                    return true;

                if (HasPassed.Count(pass => pass) >= 3)
                    return FinalizeBettingAfterThreePasses();
            }

            return false;
        }

        private bool HandlePlayerBets(int playerIndex)
        {
            while (true)
            {
                string prompt = $"{_players[playerIndex].Name}, enter a bet (between {MinimumBet}-{MaximumBet}, " +
                              $"intervals of {BetIncrement}) or 'pass': ";
                string betInput = _ui.GetUserInput(prompt).ToLower();

                if (betInput == "pass")
                {
                    HandlePassInput(playerIndex);
                    return false;
                }

                if (int.TryParse(betInput, out int bet) && IsValidBet(bet))
                {
                    return HandleValidBet(playerIndex, bet);
                }

                _ui.DisplayMessage("Invalid bet, please try again");
            }
        }

        private void HandlePassInput(int playerIndex)
        {
            _ui.DisplayFormattedMessage("\n{0} passed\n", _players[playerIndex].Name);
            HasPassed[playerIndex] = true;
            Bets[playerIndex] = -1;
        }

        private bool IsValidBet(int bet)
        {
            return bet >= MinimumBet && 
                   bet <= MaximumBet && 
                   bet % BetIncrement == 0 && 
                   !Bets.Contains(bet);
        }

        private bool HandleValidBet(int playerIndex, int bet)
        {
            Bets[playerIndex] = bet;
            PlayerHasBet[playerIndex] = true;
            CurrentWinningBid = bet;
            CurrentWinningBidIndex = playerIndex;
            _ui.DisplayMessage("");

            if (bet == MaximumBet)
            {
                FinalizeBettingAfterMaximumBet(playerIndex);
                return true;
            }

            return false;
        }

        private void FinalizeBettingAfterMaximumBet(int playerIndex)
        {
            _ui.DisplayFormattedMessage("{0} bet {1}. Betting round ends.\n", 
                _players[playerIndex].Name, MaximumBet);

            for (int otherPlayerIndex = 0; otherPlayerIndex < _players.Count; otherPlayerIndex++)
            {
                if (otherPlayerIndex != playerIndex && !HasPassed[otherPlayerIndex])
                {
                    HasPassed[otherPlayerIndex] = true;
                    Bets[otherPlayerIndex] = -1;
                }
            }
        }

        private bool FinalizeBettingAfterThreePasses()
        {
            _ui.DisplayMessage("Three players have passed, betting round ends");
            
            // If all players passed except one, that player gets the minimum bet
            if (Bets.All(b => b <= 0))
            {
                int lastPlayerIndex = (_dealerIndex + 1) % _players.Count;
                PlayerHasBet[lastPlayerIndex] = true;
                CurrentWinningBid = MinimumBet;
                CurrentWinningBidIndex = lastPlayerIndex;
                Bets[lastPlayerIndex] = MinimumBet;
            }

            return true;
        }

        private void DisplayBettingResults()
        {
            _ui.DisplayMessage("\nBetting round complete, here are the results:");
            for (int i = 0; i < _players.Count; i++)
            {
                string result;
                if (HasPassed[i])
                    result = PlayerHasBet[i] ? "Passed after betting" : "Passed";
                else
                    result = $"Bet {Bets[i]}";

                _ui.DisplayFormattedMessage("{0} : {1}", _players[i].Name, result);
            }
        }

        private void DetermineWinningBid()
        {
            CurrentWinningBid = Bets.Max();
            CurrentWinningBidIndex = Bets.IndexOf(CurrentWinningBid);
            _ui.DisplayFormattedMessage("\n{0} won the bid with {1}.", 
                _players[CurrentWinningBidIndex].Name, CurrentWinningBid);
            _ui.DisplayMessage("\n#########################\n");
        }
    }
}