using DeuxCentsCardGame.Interfaces.Models;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Interfaces.Validators;

namespace DeuxCentsCardGame.Models
{
    [Serializable]
    public class Deck : IDeck
    {
        private readonly ICardUtility _cardUtility;
        private readonly IGameValidator _gameValidator;
        public List<Card> Cards { get; set; }
        
        public Deck(ICardUtility cardUtility, IGameValidator gameValidator)
        {
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            _gameValidator = gameValidator ?? throw new ArgumentNullException(nameof(gameValidator)); 
            Cards = [];
            InitializeCards();
        }

        private void InitializeCards()
        {
            var cardSuits = _cardUtility.GetAllCardSuits();
            var cardFaces = _cardUtility.GetAllCardFaces();
            var cardFaceValues = _cardUtility.GetCardFaceValues();
            var cardPointValues = _cardUtility.GetCardPointValues();
        
            foreach (CardSuit cardSuit in cardSuits)
            {
                for (int cardIndex = 0; cardIndex < cardFaces.Length; cardIndex++)
                {
                    var suit = cardSuit;
                    var face = cardFaces[cardIndex];
                    var faceValue = cardFaceValues[cardIndex];
                    var pointValue = cardPointValues[cardIndex];

                    _gameValidator.ValidateCard(suit, face, faceValue, pointValue);
                    Cards.Add(new Card(suit, face, faceValue, pointValue));
                }
            }
        }
    }
}