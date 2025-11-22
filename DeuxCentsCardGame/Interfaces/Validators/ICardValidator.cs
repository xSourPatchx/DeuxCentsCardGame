using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Validators
{
    public interface ICardValidator
    {
        void ValidateCard(CardSuit suit, CardFace face, int faceValue, int pointValue);
        bool IsPlayableCard(Card card, CardSuit? leadingSuit, List<Card> hand);
    }
}
