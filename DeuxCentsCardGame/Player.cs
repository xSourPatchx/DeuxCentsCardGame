namespace DeuxCentsCardGame
{
    public class Player(string name) : IPlayer
    {
        public string Name { get; } = name;
        public List<Card> Hand { get; } = [];

        public void AddCard(Card card)
        {
            Hand.Add(card);
        }

        public void RemoveCard(Card card)
        {
            Hand.Remove(card);
        }
    }
}