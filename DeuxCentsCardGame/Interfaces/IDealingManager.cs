using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces
{
    public interface IDealingManager
    {
        void DealCards(Deck deck, List<Player> players);
        void RaiseCardsDealtEvent(List<Player> players, int dealerIndex);
        int RotateDealerIndex(int currentDealerIndex, int totalPlayers);
    }
}
