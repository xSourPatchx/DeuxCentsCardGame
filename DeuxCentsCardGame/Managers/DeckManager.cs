using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Interfaces.Validators;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Managers
{
    public class DeckManager : IDeckManager
    { 
        private readonly IGameEventManager _eventManager;
        private readonly IRandomService _randomService;
        private readonly ICardUtility _cardUtility;
        private readonly IGameValidator _gameValidator;
        private Deck _currentDeck;

        public Deck CurrentDeck 
        { 
            get { return _currentDeck; } 
        }

        public DeckManager(
            IGameEventManager eventManager, 
            IRandomService randomService, 
            ICardUtility cardUtility, 
            IGameValidator gameValidator)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            _gameValidator = gameValidator ?? throw new ArgumentNullException(nameof(gameValidator));
            _currentDeck = new Deck(_cardUtility, _gameValidator);
        }

        public async Task ResetDeck()
        { 
            _currentDeck = new Deck(_cardUtility, _gameValidator);
            await Task.CompletedTask;
        }

        public async Task ShuffleDeck()
        {
            await PerformFisherYatesShuffle();
            await RaiseDeckShuffled();
        }

        private async Task PerformFisherYatesShuffle()
        {
            var cards = _currentDeck.Cards;

            for (int cardIndex = 0; cardIndex < cards.Count; cardIndex++)
            {
                int randomCardIndex = _randomService.Next(cardIndex, cards.Count);
                Card temp = cards[randomCardIndex];
                cards[randomCardIndex] = cards[cardIndex];
                cards[cardIndex] = temp;

                // Small delay for shuffle animation in Unity
                if (cardIndex % 5 == 0) // Every 5 cards
                {
                    await Task.Delay(5);
                }
            }
        }

        private async Task RaiseDeckShuffled()
        {
            await _eventManager.RaiseDeckShuffled("Deck has been shuffled");
        }

        public async Task CutDeck(int cutPosition)
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

            await Task.CompletedTask;
        }
    }
}