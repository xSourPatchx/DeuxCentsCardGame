namespace DeuxCentsCardGame
{
    public enum CardSuit { Clubs, Diamonds, Hearts, Spades }
    public enum CardFace { Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public class Card : ICard
    {
        private const int MinCardFaceValue = 1;
        private const int MaxCardFaceValue = 10;
        private static readonly int[] ValidPointValues = { 0, 5, 10 };

        // card fields
        public CardSuit CardSuit { get; init; }
        public CardFace CardFace { get; init; }
        public int CardFaceValue { get; init; }
        public int CardPointValue { get; init; }

        public Card(CardSuit cardSuit, CardFace cardFace, int cardFaceValue, int cardPointValue)
        {
            ValidateConstructorArguments(cardSuit, cardFace, cardFaceValue, cardPointValue);
        
            CardSuit = cardSuit;
            CardFace = cardFace;
            CardFaceValue = cardFaceValue;
            CardPointValue = cardPointValue;
        }

        private void ValidateConstructorArguments(CardSuit suit, CardFace face, int faceValue, int pointValue)
        {
            if (!Enum.IsDefined(typeof(CardSuit), suit))
                throw new ArgumentException($"Invalid card suit: {suit}", nameof(suit));

            if (!Enum.IsDefined(typeof(CardFace), face))
                throw new ArgumentException($"Invalid card face: {face}", nameof(face));
            
            if (faceValue is < MinCardFaceValue or > MaxCardFaceValue)
                throw new ArgumentOutOfRangeException(nameof(faceValue), 
                $"Invalid card face value, must be between {MinCardFaceValue}-{MaxCardFaceValue}. faceValue : {faceValue}");
                
            if (Array.IndexOf(ValidPointValues, pointValue) == -1)
                throw new ArgumentOutOfRangeException(nameof(pointValue), 
                    $"Invalid card point value, must be {string.Join(", ", ValidPointValues)}. pointValue: {pointValue}");

            ValidateFacePointConsistency(face, pointValue);
        }

        private static void ValidateFacePointConsistency(CardFace face, int pointValue)
        {
            var expectedPointValue = face switch
            {
                CardFace.Five => 5,
                CardFace.Ten or CardFace.Ace => 10,
                _ => 0
            };

            if (pointValue != expectedPointValue)
                throw new ArgumentException(
                    $"Invalid point value {pointValue} for card face {face}. Expected {expectedPointValue}");
        }

        // Helper methods for game logic
        public bool IsTrump(CardSuit? trumpSuit)
        {
            return trumpSuit.HasValue && CardSuit == trumpSuit.Value;
        }

        public override string ToString() =>
            $"{GetCardFaceString(CardFace)} of {CardSuit.ToString().ToLower()} ({CardPointValue} pts)";

        private static string GetCardFaceString(CardFace face) => face switch
        {  
                CardFace.Jack => "J",
                CardFace.Queen => "Q",
                CardFace.King => "K",
                CardFace.Ace => "A",
                _ => ((int)face + 5).ToString()
        };
    }
}