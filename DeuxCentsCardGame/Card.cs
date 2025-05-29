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

        public bool IsTrump(CardSuit? trumpSuit)
        {
            return trumpSuit.HasValue && CardSuit == trumpSuit.Value;
        }

        public bool IsSameSuit(Card otherCard)
        {
            return CardSuit == otherCard.CardSuit;
        }

        public bool CanBePlayed(CardSuit? leadingSuit, List<Card> hand)
        {
            // Can always play if no leading suit established
            if (!leadingSuit.HasValue)
                return true; 

            // Can play if CardSuit matches leading suit
            if (CardSuit == leadingSuit.Value)
                return true;
            
            // Can play if player hand has no cards of leading suit
            // below can be used to simplify to one line
            // return hand.Any(card => card.CardSuit == leadingSuit.Value);
            foreach(Card card in hand)
            {
                if (card.CardSuit == leadingSuit.Value)
                    return false;
            }
            return true;
        }

        // left off here, continue with logic, look at Game class for reference
        public bool Beats(Card otherCard, CardSuit? trumpSuit, CardSuit? leadingSuit)
        {
            // first case
            // Check if the current card is a trump card AND the winning card is not a trump card
            if (!otherCard.IsTrump(trumpSuit) && IsTrump(trumpSuit))
                return true;

            // second case
            // Check if both cards are trump cards or both are not trump cards
            if (IsSameSuit(otherCard))
            {
                return CardFaceValue > otherCard.CardFaceValue;
            }

            // third case
            // If both cards aren't trump and different suits, card of leading suit wins
            if (leadingSuit.HasValue && CardSuit == leadingSuit.Value && otherCard.CardSuit != leadingSuit.Value)
                return true;

            // fourth case - not really needed since if we get to this point, it will end up false anyways
            // If both cards are non-trump, different suits, and neither matches leading suit,
            // first card played wins (return false since otherCard was played first)
            if (leadingSuit.HasValue && CardSuit != leadingSuit.Value && otherCard.CardSuit != leadingSuit.Value)
                return false;

            return false;
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