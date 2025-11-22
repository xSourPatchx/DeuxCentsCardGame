using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Controllers
{
    public interface IRoundController
    {
        int DealerIndex { get; }
        CardSuit? TrumpSuit { get; }
        void InitializeRound(int roundNumber);
        void PrepareRound();
        void ExecuteBettingPhase();
        void SelectTrump();
        void FinalizeRound(int roundNumber);
        int GetStartingPlayerIndex();
    }
}