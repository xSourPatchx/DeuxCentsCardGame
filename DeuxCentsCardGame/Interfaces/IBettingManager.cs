namespace DeuxCentsCardGame.Interfaces
{
    public interface IBettingManager
    { 
        int CurrentWinningBid { get; }
        int CurrentWinningBidIndex { get; }
        bool IsBettingRoundComplete { get; }
        void ExecuteBettingRound();
        void ResetBettingRound();
    }
}
