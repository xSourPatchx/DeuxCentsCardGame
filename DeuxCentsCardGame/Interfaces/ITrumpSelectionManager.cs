using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces
{
    public interface ITrumpSelectionManager
    {
        CardSuit SelectTrumpSuit(Player winningBidder);
    }
}
