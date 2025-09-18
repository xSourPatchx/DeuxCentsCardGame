using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces
{
    public interface IDeck
    {
        List<Card> Cards { get; }
        // void ShuffleDeck();
    }
}
