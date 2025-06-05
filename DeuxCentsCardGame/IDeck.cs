namespace DeuxCentsCardGame
{
    public interface IDeck
    {
        List<Card> Cards { get; }
        void ShuffleDeck();
    }
}
