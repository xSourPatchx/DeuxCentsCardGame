using DeuxCentsCardGame.Constants;
using DeuxCentsCardGame.Interfaces.Models;
using DeuxCentsCardGame.Interfaces.Services;

namespace DeuxCentsCardGame.Models
{
    public enum CardSuit { Clubs, Diamonds, Hearts, Spades }
    public enum CardFace { Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public class Card : ICard
    {
        private readonly ICardUtility _cardUtility;

        // card fields
        public CardSuit CardSuit { get; init; }
        public CardFace CardFace { get; init; }
        public int CardFaceValue { get; init; }
        public int CardPointValue { get; init; }

        public Card(CardSuit cardSuit, CardFace cardFace, int cardFaceValue, int cardPointValue, ICardUtility cardUtility)
        {
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
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
            
            if (faceValue < GameConstants.MINIMUM_CARD_FACE_VALUE || faceValue > GameConstants.MAXIMUM_CARD_FACE_VALUE)
                throw new ArgumentOutOfRangeException(nameof(faceValue), 
                $"Invalid card face value, must be between {GameConstants.MINIMUM_CARD_FACE_VALUE}-{GameConstants.MAXIMUM_CARD_FACE_VALUE}. faceValue : {faceValue}");

            if (Array.IndexOf(_cardUtility.GetCardPointValues(), pointValue) == -1)
                throw new ArgumentOutOfRangeException(nameof(pointValue),
                    $"Invalid card point value, must be {string.Join(", ", _cardUtility.GetCardPointValues())}. pointValue: {pointValue}");

            ValidateFacePointConsistency(face, pointValue);
        }

        private void ValidateFacePointConsistency(CardFace face, int pointValue)
        {
            var expectedPointValue = face switch
            {
                CardFace.Five => GameConstants.CARD_POINT_VALUE_FIVE,
                CardFace.Ten or CardFace.Ace => GameConstants.CARD_POINT_VALUE_TEN,
                _ => GameConstants.CARD_POINT_VALUE_ZERO
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

        public bool IsPlayableCard(CardSuit? leadingSuit, List<Card> hand)
        {
            // Can play if no leading suit established
            if (!leadingSuit.HasValue)
                return true;

            // Can play if CardSuit matches leading suit
            if (CardSuit == leadingSuit.Value)
                return true;

            // Can play if no card of leading suit in hand
            return !hand.Any(card => card.CardSuit == leadingSuit.Value);
        }

        public bool WinsAgainst(Card otherCard, CardSuit? trumpSuit, CardSuit? leadingSuit)
        {
            bool thisCardIsTrump = IsTrump(trumpSuit);
            bool otherCardIsTrump = otherCard.IsTrump(trumpSuit);

            // Handle trump card scenarios
            if (thisCardIsTrump || otherCardIsTrump)
                return HandleTrumpComparison(otherCard, thisCardIsTrump, otherCardIsTrump);

            // Handle leading suit scenarios
            if (leadingSuit.HasValue)
                return HandleLeadingSuitComparison(otherCard, leadingSuit.Value);

            // Handle same suit comparison
            return HandleSameSuitComparison(otherCard);
        }

        private bool HandleTrumpComparison(Card otherCard, bool thisCardIsTrump, bool otherCardIsTrump)
        {
            // Only this card is trump, we win
            if (thisCardIsTrump && !otherCardIsTrump)
                return true;

            // Only other card is trump, we lose
            if (!thisCardIsTrump && otherCardIsTrump)
                return false;

            // Both are trump, higher face value wins
            return CardFaceValue > otherCard.CardFaceValue;
        }

        private bool HandleLeadingSuitComparison(Card otherCard, CardSuit leadingSuit)
        {
            bool thisCardMatchesLeading = CardSuit == leadingSuit;
            bool otherCardMatchesLeading = otherCard.CardSuit == leadingSuit;

            // Only this card matches leading suit, we win
            if (thisCardMatchesLeading && !otherCardMatchesLeading)
                return true;

            // Only other card matches leading suit, we lose
            if (!thisCardMatchesLeading && otherCardMatchesLeading)
                return false;

            // Both match or neither matches - compare if same suit
            return HandleSameSuitComparison(otherCard);
        }

        private bool HandleSameSuitComparison(Card otherCard)
        {
            // Only compare face values if cards are same suit
            if (IsSameSuit(otherCard))
                return CardFaceValue > otherCard.CardFaceValue;
            
            // default case
            return false;
        }

        public override string ToString() =>
            $"{_cardUtility.FormatCardFace(CardFace)} of {CardSuit.ToString().ToLower()} ({CardPointValue} pts)";
    }
}