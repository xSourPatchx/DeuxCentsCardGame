using Microsoft.Win32.SafeHandles;

namespace DeuxCentsCardGame
{
    public class ConsoleGameView
    {
        public static void ClearScreen()
        {
            Console.Clear();
        }
        public static void DisplayHand(IPlayer player)
        {
            Console.WriteLine($"{player.Name}'s hand:");
            for (int p = 0; p < player.Hand.Count; p++)
            {
                Thread.Sleep(20);
                Console.WriteLine($"{p}: {player.Hand[p]}");
            }
            Thread.Sleep(300);
        }
        public static void DisplayAllPlayersHand(IPlayer playerOne, IPlayer playerTwo, IPlayer playerThree, IPlayer playerFour)
        {
            DisplayHand(playerOne);
            DisplayHand(playerTwo);
            DisplayHand(playerThree);
            DisplayHand(playerFour);
        }

        public static void DisplayPlayerHandQuadrant(IPlayer player, int left, int top)
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

        public static void DisplayAllPlayersHandQuadrant(IPlayer playerOne, IPlayer playerTwo, IPlayer playerThree, IPlayer playerFour)
        {
            DisplayPlayerHandQuadrant(playerOne, 0, 4);
            DisplayPlayerHandQuadrant(playerTwo, Console.WindowWidth / 2, 4);
            DisplayPlayerHandQuadrant(playerThree, 0, (Console.WindowHeight / 2) + 1);
            DisplayPlayerHandQuadrant(playerFour, Console.WindowWidth / 2, (Console.WindowHeight / 2) + 1);
            Console.WriteLine("\n#########################\n");
        }

        public static void DisplayAllHands(List<Player> _players, int _dealerIndex)
        {
            DisplayAllPlayersHandQuadrant(_players[(_dealerIndex) % _players.Count],
                                                 _players[(_dealerIndex + 1) % _players.Count],
                                                 _players[(_dealerIndex + 2) % _players.Count],
                                                 _players[(_dealerIndex + 3) % _players.Count]);
        }
    }
}
