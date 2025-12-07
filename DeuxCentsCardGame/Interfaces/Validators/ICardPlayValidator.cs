using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Validators
{
    public interface ICardPlayValidator
    {
        Task<Card> GetValidCardFromPlayer(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit);
        List<Card> GetValidCards(List<Card> hand, CardSuit? leadingSuit);
        List<int> GetValidCardIndices(List<Card> hand, CardSuit? leadingSuit);
        bool IsCardIndexValid(List<Card> hand, int cardIndex, CardSuit? leadingSuit);
    }
}