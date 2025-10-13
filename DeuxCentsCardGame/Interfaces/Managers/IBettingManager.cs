namespace DeuxCentsCardGame.Interfaces.Managers
{
    public interface IBettingManager
    { 
        int CurrentWinningBid { get; }
        int CurrentWinningBidIndex { get; }
        bool IsBettingRoundComplete { get; }
        void ExecuteBettingRound();
        void ResetBettingRound();
        void UpdateDealerIndex(int newDealerIndex);
    }
}