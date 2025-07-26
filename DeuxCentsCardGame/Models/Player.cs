using DeuxCentsCardGame.Interfaces;

namespace DeuxCentsCardGame.Models
{
    public class Player(string name) : IPlayer
    {
        public string Name { get; } = name;
        public List<Card> Hand { get; } = [];
        public bool HasBet { get; set; }
        public bool HasPassed { get; set; }
        public int CurrentBid { get; set; }

        public void AddCard(Card card)
        {
            Hand.Add(card);
        }

        public void RemoveCard(Card card)
        {
            Hand.Remove(card);
        }

        public void ResetBettingState()
        {
            HasBet = false;
            HasPassed = false;
            CurrentBid = 0;
        }
    }
}