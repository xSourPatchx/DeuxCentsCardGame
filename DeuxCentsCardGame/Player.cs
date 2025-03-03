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

        // add card method
        public void AddCard(Card card)
        {
            Hand.Add(card);
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
            Thread.Sleep(300);
            Console.WriteLine();
        }

        public static void DisplayPlayerHand(Player player, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.WriteLine($"{player.Name}'s hand:");
            for (int i = 0; i < player.Hand.Count; i++)
            {
                Thread.Sleep(20);
                Console.SetCursorPosition(left, top + i + 1);
                Console.WriteLine($"{i} : {player.Hand[i]}");
            }
            Thread.Sleep(300);
        }
    }
}

