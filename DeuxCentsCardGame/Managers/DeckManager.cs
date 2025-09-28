using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class DeckManager : IDeckManager
    { 
        private readonly GameEventManager _eventManager;
        private readonly IRandomService _randomProvider;
        private Deck _currentDeck;

        public Deck CurrentDeck 
        { 
            get { return _currentDeck; } 
        }

        public DeckManager(GameEventManager eventManager, IRandomService randomProvider)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
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
                int randomCardIndex = _randomProvider.Next(cardIndex, cards.Count);
                Card temp = cards[randomCardIndex];
                cards[randomCardIndex] = cards[cardIndex];
                cards[cardIndex] = temp;
            }

            // Could raise a DeckShuffled event if needed
            // _eventManager.RaiseDeckShuffled();
        }

    }
}