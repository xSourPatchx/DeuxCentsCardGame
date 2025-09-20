using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces
{
    public interface IDeckManager
    {
        Deck CurrentDeck { get; }
        void ResetDeck();
        void ShuffleDeck();
    }
}
