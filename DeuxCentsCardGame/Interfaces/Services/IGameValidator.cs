using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Validators
{
    public interface IGameValidator
    {
        // Betting Validation
        bool IsValidBet(int bet);
        bool IsBetInValidRange(int bet);
        bool IsBetValidIncrement(int bet);
        bool IsBetUnique(int bet);
        bool IsMaximumBet(int bet);
        bool HasMinimumPlayersPassed();
        bool HasPlayerPassed(Player player);
        bool IsPassInput(string input);

        // Card Play Validation
        Task<Card> GetValidCardFromPlayer(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit);
        List<Card> GetValidCards(List<Card> hand, CardSuit? leadingSuit);
        List<int> GetValidCardIndices(List<Card> hand, CardSuit? leadingSuit);
        bool IsCardIndexValid(List<Card> hand, int cardIndex, CardSuit? leadingSuit);

        // Card Validation
        void ValidateCard(CardSuit suit, CardFace face, int faceValue, int pointValue);
        bool IsPlayableCard(Card card, CardSuit? leadingSuit, List<Card> hand);
    }
}