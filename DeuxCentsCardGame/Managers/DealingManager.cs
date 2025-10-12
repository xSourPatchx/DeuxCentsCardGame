using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class DealingManager : IDealingManager
    {
        private readonly IGameEventManager _eventManager;

        public DealingManager(IGameEventManager eventManager)
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