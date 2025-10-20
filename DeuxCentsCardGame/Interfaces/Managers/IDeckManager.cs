using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Managers
{
    public interface IDeckManager
    {
        Deck CurrentDeck { get; }
        void ResetDeck();
        void ShuffleDeck();
        void CutDeck(int cutPosition);
    }
}
