using DeuxCentsCardGame.Interfaces.Models;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Validators;

namespace DeuxCentsCardGame.Models
{
    [Serializable]
    public class Deck : IDeck
    {
        private readonly ICardUtility _cardUtility;
        private readonly CardValidator _cardValidator;
        public List<Card> Cards { get; set; }
        
        public Deck(ICardUtility cardUtility, CardValidator cardValidator)
        {
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
            _cardValidator = cardValidator ?? throw new ArgumentNullException(nameof(cardValidator)); 
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

                    _cardValidator.ValidateCard(suit, face, faceValue, pointValue);
                    Cards.Add(new Card(suit, face, faceValue, pointValue));
                }
            }
        }
    }
}