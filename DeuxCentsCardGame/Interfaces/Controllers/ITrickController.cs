using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Controllers
{
    public interface ITrickController
    {
        Task PlayAllTricks(int startingPlayerIndex, CardSuit? trumpSuit);
        Task ExecuteSingleTrick(int trickNumber, CardSuit? trumpSuit);
    }
}