using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class DeckManager : IDeckManager
    { 
        private readonly IGameEventManager _eventManager;
        private readonly IRandomService _randomService;
        private Deck _currentDeck;

        public Deck CurrentDeck 
        { 
            get { return _currentDeck; } 
        }

        public DeckManager(IGameEventManager eventManager, IRandomService randomService)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
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
                int randomCardIndex = _randomService.Next(cardIndex, cards.Count);
                Card temp = cards[randomCardIndex];
                cards[randomCardIndex] = cards[cardIndex];
                cards[cardIndex] = temp;
            }

            _eventManager.RaiseDeckShuffled("Deck has been shuffled");

        }
        public void CutDeck(int cutPosition)
            {
                if (cutPosition < 1 || cutPosition >= _currentDeck.Cards.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(cutPosition), 
                        $"Cut position must be between 1 and {_currentDeck.Cards.Count - 1}");
                }

                var cards = _currentDeck.Cards; 
                var topPortion = cards.Skip(cutPosition).ToList();               
                var bottomPortion = cards.Take(cutPosition).ToList();
                
                _currentDeck.Cards = topPortion.Concat(bottomPortion).ToList();
            }
    }
}