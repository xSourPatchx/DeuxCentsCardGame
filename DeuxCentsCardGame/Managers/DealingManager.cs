using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class DealingManager
    {
        private readonly GameEventManager _eventManager;

        public DealingManager(GameEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public void DealCards(Deck deck, List<Player> players)
        {
            foreach (Player player in players)
            {
                player.Hand.Clear();
            }

            for (int cardIndex = 0; cardIndex < deck.Cards.Count; cardIndex++)
            {
                players[cardIndex % players.Count].AddCard(deck.Cards[cardIndex]);
            }
        }

        public void RaiseCardsDealtEvent(List<Player> players, int dealerIndex)
        {
            _eventManager.RaiseCardsDealt(players, dealerIndex);
        }

        // not used yet
        public int RotateDealerIndex(int currentDealerIndex, int totalPlayers)
        {
            return (currentDealerIndex + 1) % totalPlayers;
        }
    
    }
}