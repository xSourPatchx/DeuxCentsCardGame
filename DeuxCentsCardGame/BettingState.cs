namespace DeuxCentsCardGame
{
    public class BettingState
    {
        // Betting constants
        public const int MinimumBet = 50;
        public const int MaximumBet = 100;
        public const int BetIncrement = 5;

        // Betting state
        public List<int> BidValue { get; set; }
        public bool[] PlayerHasBet { get; set; }
        bool[] PlayerHasPassed { get; set; } 
        bool IsBettingRoundComplete { get; set; }

        private int _currentWinningBid; // not sure
        private int _currentWinningBidIndex; // not sure

        // betting methods
        
        // ProcessBettingRound

        // ProcessBettingRound

        // ProcessPlayerBets

        // ProcessPassInput

        // IsValidBet

        // HandleMaximumBet

        // HandleThreePlayerPassed

        // DisplayBettingResults


    }
}