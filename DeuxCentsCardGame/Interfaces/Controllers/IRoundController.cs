using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Controllers
{
    public interface IRoundController
    {
        int DealerIndex { get; }
        CardSuit? TrumpSuit { get; }
        Task InitializeRound(int roundNumber);
        Task PrepareRound();
        Task ExecuteBettingPhase();
        Task SelectTrump();
        Task FinalizeRound(int roundNumber);
        int GetStartingPlayerIndex();
    }
}