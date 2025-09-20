using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class DeckManager : IDeckManager
    { 
        private readonly GameEventManager _eventManager;
        private Deck _currentDeck;

        public Deck CurrentDeck 
        { 
            get { return _currentDeck; } 
        }

        public DeckManager(GameEventManager eventManager)
        {
            _eventManager = eventManager;
            _currentDeck = new Deck();
        }

        public void ResetDeck()
        { 
            _currentDeck = new Deck();
        }

        public void ShuffleDeck()
        {
            Console.WriteLine("Shuffling cards...\n");
            var cards = _currentDeck.Cards;
            
            for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++)
            {
                int randomCardIndex = Random.Shared.Next(cardIndex, cards.Count);
                Card temp = cards[randomCardIndex];
                cards[randomCardIndex] = cards[cardIndex];
                cards[cardIndex] = temp;
            }

            // Could raise a DeckShuffled event if needed
            // _eventManager.RaiseDeckShuffled();
        }

    }
}