using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Managers
{
    public interface ITrumpSelectionManager
    {
        Task<CardSuit> SelectTrumpSuit(Player winningBidder);
    }
}
