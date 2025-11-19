using DeuxCentsCardGame.Constants;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Validators
{
    public class CardValidator
    {
        private readonly ICardUtility _cardUtility;

        public CardValidator(ICardUtility cardUtility)
        {
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));
        }

        public void ValidateCard(CardSuit suit, CardFace face, int faceValue, int pointValue)
        {
            ValidateSuit(suit);
            ValidateFace(face);
            ValidateFaceValue(faceValue);
            ValidatePointValue(pointValue);
            ValidateFacePointConsistency(face, pointValue);            
        }

        private void ValidateSuit(CardSuit suit)
        {
            if (!Enum.IsDefined(typeof(CardSuit), suit))
                throw new ArgumentException($"Invalid card suit: {suit}", nameof(suit));
        }

        private void ValidateFace(CardFace face)
        {
            if (!Enum.IsDefined(typeof(CardFace), face))
                throw new ArgumentException($"Invalid card face: {face}", nameof(face));
        }

        private void ValidateFaceValue(int faceValue)
        {
            if (faceValue < GameConstants.MINIMUM_CARD_FACE_VALUE || faceValue > GameConstants.MAXIMUM_CARD_FACE_VALUE)
                throw new ArgumentOutOfRangeException(nameof(faceValue),
                $"Invalid card face value, must be between {GameConstants.MINIMUM_CARD_FACE_VALUE}-{GameConstants.MAXIMUM_CARD_FACE_VALUE}. faceValue : {faceValue}");
        }

        private void ValidatePointValue(int pointValue)
        {
            if (Array.IndexOf(_cardUtility.GetCardPointValues(), pointValue) == -1)
                throw new ArgumentOutOfRangeException(nameof(pointValue),
                    $"Invalid card point value, must be {string.Join(", ", _cardUtility.GetCardPointValues())}. pointValue: {pointValue}");
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

        public bool IsPlayableCard(Card card, CardSuit? leadingSuit, List<Card> hand)
        {
            // Can play if no leading suit established
            if (!leadingSuit.HasValue)
                return true;

            // Can play if card matches leading suit
            if (card.CardSuit == leadingSuit.Value)
                return true;

            // Can play if no card of leading suit in hand
            return !hand.Any(card => card.CardSuit == leadingSuit.Value);
        }
    }
}