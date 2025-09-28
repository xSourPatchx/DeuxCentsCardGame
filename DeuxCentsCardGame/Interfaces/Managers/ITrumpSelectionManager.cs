using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Managers
{
    public interface ITrumpSelectionManager
    {
        CardSuit SelectTrumpSuit(Player winningBidder);
    }
}
