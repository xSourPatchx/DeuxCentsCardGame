using DeuxCentsCardGame.Interfaces.Models;

namespace DeuxCentsCardGame.Models
{
    [Serializable]
    public class Player : IPlayer
    {
        public string Name { get; }
        public List<Card> Hand { get; }
        public bool HasBet { get; set; }
        public bool HasPassed { get; set; }
        public int CurrentBid { get; set; }
        public PlayerType PlayerType { get; set; }

        public Player(string name, PlayerType playerType = PlayerType.Human)
        {
            Name = name;
            PlayerType = playerType;
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

        public void ResetBettingState()
        {
            HasBet = false;
            HasPassed = false;
            CurrentBid = 0;
        }

        public bool IsHuman()
        {
            return PlayerType == PlayerType.Human;
        }

        public bool IsAI()
        {
            return PlayerType == PlayerType.AI;
        }
    }
}