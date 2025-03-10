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

        public static void DisplayHand(Player player)
        {
            Console.WriteLine($"{player.Name}'s hand:");
            for (int i = 0; i < player.Hand.Count; i++)
            {
                Thread.Sleep(20);
                Console.WriteLine($"{i}: {player.Hand[i]}");
            }
            Thread.Sleep(300);
            Console.WriteLine();
        }

        public static void DisplayAllPlayersHand(Player playerOne, Player playerTwo, Player playerThree, Player playerFour)
        {
            Player.DisplayHand(playerOne);
            Player.DisplayHand(playerTwo);
            Player.DisplayHand(playerThree);
            Player.DisplayHand(playerFour);
        }

        public static void DisplayPlayerHandQuadrant(Player player, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.WriteLine($"{player.Name}'s hand:");
            for (int i = 0; i < player.Hand.Count; i++)
            {
                Thread.Sleep(20);
                Console.SetCursorPosition(left, top + i + 1);
                Console.WriteLine($"{i} : {player.Hand[i]}");
            }
            Thread.Sleep(200);
        }

        public static void DisplayAllPlayersHandQuadrant(Player playerOne, Player playerTwo, Player playerThree, Player playerFour)
        {
            Player.DisplayPlayerHandQuadrant(playerOne, 0, 0);
            Console.WriteLine("\n#########################\n");
            Player.DisplayPlayerHandQuadrant(playerTwo, Console.WindowWidth / 2, 0);
            Console.WriteLine("\n#########################\n");
            Player.DisplayPlayerHandQuadrant(playerThree, 0, Console.WindowHeight / 2);
            Console.WriteLine("\n#########################\n");
            Player.DisplayPlayerHandQuadrant(playerFour, Console.WindowWidth / 2, Console.WindowHeight / 2); 
            Console.WriteLine("\n#########################\n");
        }
    }
}