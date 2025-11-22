using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Controllers
{
    public interface ITrickController
    {
        void PlayAllTricks(int startingPlayerIndex, CardSuit? trumpSuit);
    }
}