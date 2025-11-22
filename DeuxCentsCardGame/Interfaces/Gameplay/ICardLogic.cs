using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Gameplay
{
    public interface ICardLogic
    {
        bool WinsAgainst(Card thisCard, Card otherCard, CardSuit? trumpSuit, CardSuit? leadingSuit);
    }
}