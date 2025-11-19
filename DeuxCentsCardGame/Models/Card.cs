using DeuxCentsCardGame.Interfaces.Models;

namespace DeuxCentsCardGame.Models
{
    [Serializable]
    public enum CardSuit { Clubs, Diamonds, Hearts, Spades }

    [Serializable]
    public enum CardFace { Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

    [Serializable]
    public class Card : ICard
    {
        // card fields
        public CardSuit CardSuit { get; init; }
        public CardFace CardFace { get; init; }
        public int CardFaceValue { get; init; }
        public int CardPointValue { get; init; }

        public Card(CardSuit cardSuit, CardFace cardFace, int cardFaceValue, int cardPointValue)
        {
            CardSuit = cardSuit;
            CardFace = cardFace;
            CardFaceValue = cardFaceValue;
            CardPointValue = cardPointValue;
        }

        public bool IsTrump(CardSuit? trumpSuit)
        {
            return trumpSuit.HasValue && CardSuit == trumpSuit.Value;
        }

        public bool IsSameSuit(Card otherCard)
        {
            return CardSuit == otherCard.CardSuit;
        }

        public override string ToString() =>
            $"{CardFace} of {CardSuit.ToString().ToLower()} ({CardPointValue} pts)";
    }
}