namespace DeuxCentsCardGame
{
    public class Player : IPlayer
    {
        public string Name { get; }
        public List<Card> Hand { get; }

        public Player(string name)
        {
            Name = name;
            Hand = [];
        }

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