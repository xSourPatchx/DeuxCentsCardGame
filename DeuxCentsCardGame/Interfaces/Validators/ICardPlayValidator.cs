using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Validators
{
    public interface ICardPlayValidator
    {
        Card GetValidCardFromPlayer(Player currentPlayer, CardSuit? leadingSuit, CardSuit? trumpSuit);
    }
}