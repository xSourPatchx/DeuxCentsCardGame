using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Managers
{
    public interface IDeckManager
    {
        Deck CurrentDeck { get; }
        Task ResetDeck();
        Task ShuffleDeck();
        Task CutDeck(int cutPosition);
    }
}
