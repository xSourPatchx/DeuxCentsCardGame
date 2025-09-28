using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Interfaces.Models
{
    public interface IDeck
    {
        List<Card> Cards { get; }
        // void ShuffleDeck();
    }
}
