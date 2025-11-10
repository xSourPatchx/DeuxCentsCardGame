using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Services
{
    public class CardUtility : ICardUtility
    {
        private readonly CardSuit[] _cardSuits =
        [
            CardSuit.Clubs,
            CardSuit.Diamonds,
            CardSuit.Hearts,
            CardSuit.Spades
        ];

        private readonly CardFace[] _cardFaces =
        [
            CardFace.Five,
            CardFace.Six,
            CardFace.Seven,
            CardFace.Eight,
            CardFace.Nine,
            CardFace.Ten,
            CardFace.Jack,
            CardFace.Queen,
            CardFace.King,
            CardFace.Ace
        ];

        private readonly int[] _cardFaceValues = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        private readonly int[] _cardPointValues = [5, 0, 0, 0, 0, 10, 0, 0, 0, 10];

        public CardSuit StringToCardSuit(string cardSuitName)
        {
            return cardSuitName.ToLower() switch
            {
                "clubs" => CardSuit.Clubs,
                "diamonds" => CardSuit.Diamonds,
                "hearts" => CardSuit.Hearts,
                "spades" => CardSuit.Spades,
                _ => throw new ArgumentException($"Invalid suit name: {cardSuitName}")
            };
        }

        public string CardSuitToString(CardSuit cardSuit)
        {
            return cardSuit.ToString().ToLower();
        }

        public CardSuit[] GetAllCardSuits() => _cardSuits;

        public CardFace[] GetAllCardFaces() => _cardFaces;

        public int[] GetCardFaceValues() => _cardFaceValues;

        public int[] GetCardPointValues() => _cardPointValues;

        public string FormatCardFace(CardFace face) => face switch
        {
            CardFace.Jack => "J",
            CardFace.Queen => "Q",
            CardFace.King => "K",
            CardFace.Ace => "A",
            _ => ((int)face + 5).ToString()
        };
    }
}