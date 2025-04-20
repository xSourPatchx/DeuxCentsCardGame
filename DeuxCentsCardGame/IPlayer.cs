namespace DeuxCentsCardGame
{
    public interface IPlayer
    {
        // Properties
        public string Name { get; }
        public List<Card> Hand { get; }

        // Core player actions (Methods)
        void AddCard(Card card);
        void RemoveCard(Card card);
    }
}
