using System;
using System.Diagnostics.Contracts;

namespace DeuxCentsCardGame
{
    public class Player
    {
        public string Name;
        public List<Card> Hand;
        public Player(string name)
        {
            Name = name;
            Hand = new List<Card>();
        }

        // remove card method
        public void RemoveCard(Card card)
        {
            Hand.Remove(card); // in original code, i remove index, not "Card" object
        }

        public void DisplayHand()
        {
            Console.WriteLine($"{Name}'s hand:");
            for (int i = 0; i < Hand.Count; i++)
            {
                Console.WriteLine($"{i}: {Hand[i]}");
            }
        }
    }
}

