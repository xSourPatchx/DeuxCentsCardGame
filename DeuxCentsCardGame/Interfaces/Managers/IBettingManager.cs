namespace DeuxCentsCardGame.Interfaces.Managers
{
    public interface IBettingManager
    { 
        int CurrentWinningBid { get; }
        int CurrentWinningBidIndex { get; }
        bool IsBettingRoundComplete { get; }
        Task ExecuteBettingRound();
        Task ResetBettingRound();
        Task UpdateDealerIndex(int newDealerIndex);
    }
}