using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Managers
{
    public interface IDealingManager
    {
        Task DealCards(Deck deck, List<Player> players);
        Task RaiseCardsDealtEvent(List<Player> players, int dealerIndex);
        int RotateDealerIndex(int currentDealerIndex, int totalPlayers);
    }
}
