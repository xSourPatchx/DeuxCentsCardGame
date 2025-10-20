using DeuxCentsCardGame.Interfaces.Models;

namespace DeuxCentsCardGame.Models
{
    public class Deck : IDeck
    {
        public List<Card> Cards { get; set; }
        
        private static readonly CardSuit[] cardSuits = 
        [
            CardSuit.Clubs, 
            CardSuit.Diamonds, 
            CardSuit.Hearts, 
            CardSuit.Spades
        ];
        
        private static readonly CardFace[] cardFaces = 
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
        private static readonly int[] cardFaceValues = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        private static readonly int[] cardPointValues = [5, 0, 0, 0, 0, 10, 0, 0, 0, 10];

        public Deck()
        {
            Cards = [];
            InitializeCards();
        }

        private void InitializeCards()
        {
            foreach (CardSuit suit in cardSuits)
            {
                for (int cardIndex = 0; cardIndex < cardFaces.Length; cardIndex++)
                {
                    Cards.Add(new Card(suit, cardFaces[cardIndex], cardFaceValues[cardIndex], cardPointValues[cardIndex]));
                }
            }
        }

        public static CardSuit[] GetCardSuits()
        {
            return cardSuits;
        }

        public static CardSuit StringToCardSuit(string cardSuitName)
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

        public static string CardSuitToString(CardSuit cardSuit)
        {
            return cardSuit.ToString().ToLower();
        }
    }
}