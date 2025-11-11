using DeuxCentsCardGame.Interfaces.Models;
using DeuxCentsCardGame.Interfaces.Services;

namespace DeuxCentsCardGame.Models
{
    [Serializable]
    public class Deck : IDeck
    {
        private readonly ICardUtility _cardUtility;
        public List<Card> Cards { get; set; }
        
        public Deck(ICardUtility cardUtility)
        {
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
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
                    Cards.Add(new Card(cardSuit, cardFaces[cardIndex], cardFaceValues[cardIndex], cardPointValues[cardIndex], _cardUtility));
                }
            }
        }
    }
}