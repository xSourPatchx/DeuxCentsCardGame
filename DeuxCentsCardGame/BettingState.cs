namespace DeuxCentsCardGame
{
    public class BettingState
    {
        // Betting constants
        public const int MinimumBet = 50;
        public const int MaximumBet = 100;
        public const int BetIncrement = 5;

        // Betting state
        public List<int> PlayerBids { get; set; }
        public bool[] PlayerHasBet { get; set; }
        public bool[] PlayerHasPassed { get; set; } 
        public int CurrentWinningBid { get; set; }
        public int CurrentWinningBidIndex { get; set; }
        public bool IsBettingRoundComplete { get; set; }

        // Betting properties
        private readonly List<Player> _players;
        private readonly IUIConsoleGameView _ui;
        private readonly int _dealerIndex;

        public BettingState(List<Player> players, IUIConsoleGameView ui, int dealerIndex)
        {
            _players = players;
            _ui = ui;
            _dealerIndex = dealerIndex;

            PlayerBids = new(new int[_players.Count]);
            PlayerHasBet = new bool[_players.Count];
            PlayerHasPassed = new bool[_players.Count];
            CurrentWinningBid = 0;
            CurrentWinningBidIndex = 0;
            IsBettingRoundComplete = false;

        }

        public void ExecuteBettingRound()
        {
            _ui.DisplayMessage("Betting round begins!\n");

            bool bettingRoundEnded = false;
            int startingIndex = _dealerIndex % _players.Count;

            while (!bettingRoundEnded)
            {
                bettingRoundEnded = HandleBettingRound(startingIndex);
            }

            DisplayBettingResults();
            DetermineWinningBid();
            IsBettingRoundComplete = true;
        }

        private bool HandleBettingRound(int startingIndex)
        {
            for (int i = 0; i < _players.Count; i++)
                {
                    int currentPlayerIndex = (startingIndex + i) % _players.Count;

                    if (PlayerHasPassed[currentPlayerIndex])
                        continue; // Skip players who have already passed
                    
                    if (HandlePlayerBets(currentPlayerIndex))
                        return true;

                    if (PlayerHasPassed.Count(pass => pass) >= 3) // End the betting round if 3 players have passed                 
                        return FinalizeBettingAfterThreePasses();
                }

                return false;
        }

        private bool HandlePlayerBets(int currentPlayerIndex)
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
                else
                {
                    _ui.DisplayMessage("Invalid bet, please try again");
                }
            }
        }
        private void HandlePassInput(int currentPlayerIndex)
        {
            _ui.DisplayFormattedMessage("\n{0} passed\n", _players[currentPlayerIndex].Name);
            PlayerHasPassed[currentPlayerIndex] = true;
            PlayerBids[currentPlayerIndex] = -1;     
        }

        private static bool IsValidBet(int bet)
        {
            return bet >= MinimumBet &&
                   bet <= MaximumBet && 
                   bet % BetIncrement == 0 && 
                   !PlayerBids.Contains(bet);
        }

        private bool HandleValidBet(int currentPlayerIndex, int bet)
        {
            PlayerBids[currentPlayerIndex] = bet;
            PlayerHasPassed[currentPlayerIndex] = true;
            _ui.DisplayMessage("");
        
            if (bet == MaximumBet)
            {
                return FinalizeBettingAfterMaximumBet(currentPlayerIndex); // End the betting round immediately
            }

            return false;
        }

        private bool FinalizeBettingAfterMaximumBet(int playerIndex)
        {
            _ui.DisplayFormattedMessage("{0} bet {1}. Betting round ends.\n", _players[playerIndex].Name, MaximumBet);
            for (int otherPlayerIndex = 0; otherPlayerIndex < _players.Count; otherPlayerIndex++)
            {
                if (otherPlayerIndex != playerIndex && !PlayerHasPassed[otherPlayerIndex])
                {
                    PlayerHasPassed[otherPlayerIndex] = true;
                    PlayerBids[otherPlayerIndex] = -1;
                }
            }
            return true;   
        }

        private bool FinalizeBettingAfterThreePasses()
        {
            _ui.DisplayMessage("Three players have passed, betting round ends");
            
            // If all players passed except one, that player gets the minimum bet
            if (PlayerBids.All(bet => bet <= 0))
            {
                int lastPlayerIndex = (_dealerIndex + 1) % _players.Count;
                PlayerHasBet[lastPlayerIndex] = true;
                CurrentWinningBid = MinimumBet;
                CurrentWinningBidIndex = lastPlayerIndex;
                PlayerBids[lastPlayerIndex] = MinimumBet;
            }

            return true;
        }

        private void DisplayBettingResults()
        {
            _ui.DisplayMessage("\nBetting round complete, here are the results:");
            for (int i = 0; i < _players.Count; i++)
            {
                string result;
                if (PlayerHasPassed[i])
                    result = PlayerHasPassed[i] ? "Passed after betting" : "Passed";
                else
                    result = $"Bet {PlayerBids[i]}";

                _ui.DisplayFormattedMessage("{0} : {1}", _players[i].Name, result);
            }
        }

        private void DetermineWinningBid()
        {
            CurrentWinningBid = PlayerBids.Max();
            CurrentWinningBidIndex = PlayerBids.IndexOf(CurrentWinningBidIndex = PlayerBids.IndexOf(CurrentWinningBid));
            _ui.DisplayFormattedMessage("\n{0} won the bid.", _players[CurrentWinningBidIndex].Name);
            _ui.DisplayMessage("\n#########################\n");
            UIConsoleGameView.DisplayHand(_players[CurrentWinningBidIndex]);
        }
    }
}