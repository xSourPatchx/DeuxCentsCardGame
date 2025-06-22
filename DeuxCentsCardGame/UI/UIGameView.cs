using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Interfaces;

namespace DeuxCentsCardGame.UI
{
    public class UIGameView : IUIGameView
    {
        private readonly IConsoleWrapper _console;

        public UIGameView() : this(new ConsoleWrapper()) { }

        public UIGameView(IConsoleWrapper console)
        {
            _console = console;
        }

        public void ClearScreen()
        {
            _console.Clear();
        }

        public void DisplayMessage(string message)
        {
            _console.WriteLine(message);
        }

        public void DisplayFormattedMessage(string format, params object[] args)
        {
            _console.WriteLine(format, args);
        }

        public string GetUserInput(string prompt)
        {
            _console.WriteLine(prompt);
            return _console.ReadLine() ?? string.Empty;
        }

        public int GetIntInput(string prompt, int min, int max)
        {
            int result;
            bool isValid;

            do
            {
                string input = GetUserInput(prompt);
                isValid = int.TryParse(input, out result) && result >= min && result <= max;

                if (!isValid)
                {
                    DisplayMessage($"Invalid input. Please enter a number between {min} and {max}.");
                }
                
            } while (!isValid);

            return result;
        }

        public string GetOptionInput(string prompt, string[] options)
        {
            string result;
            bool isValid;

            do
            {
                result = GetUserInput(prompt).ToLower();
                isValid = options.Contains(result, StringComparer.OrdinalIgnoreCase);
                
                if (!isValid)
                {
                    DisplayMessage($"Invalid input. Please enter one of: {string.Join(", ", options)}");
                }
            } while (!isValid);

            return result;
        }

        public void WaitForUser(string message = "Press any key to continue...")
        {
            DisplayMessage(message);
            _console.ReadKey();
        }

        // uncomment once GameState is implemented
        // public void ShowGameState(GameState state)
        // {
        //     DisplayMessage("\n###### Game State #######");
        //     DisplayMessage($"Current Round: {state.CurrentRound}");
        //     DisplayMessage($"Team 1 Score: {state.TeamOnePoints}");
        //     DisplayMessage($"Team 2 Score: {state.TeamTwoPoints}");
        //     // add more game state

        //     DisplayMessage("#########################\n");
        // }

        public static void DisplayHand(IPlayer player)
        {
            Console.WriteLine($"{player.Name}'s hand:");
            for (int playerIndex = 0; playerIndex < player.Hand.Count; playerIndex++)
            {
                Console.WriteLine($"{playerIndex}: {player.Hand[playerIndex]}");
            }
        }

        private static void DisplayAllPlayersHand(IPlayer playerOne, IPlayer playerTwo, IPlayer playerThree, IPlayer playerFour)
        {
            DisplayHand(playerOne);
            DisplayHand(playerTwo);
            DisplayHand(playerThree);
            DisplayHand(playerFour);
        }

        private static void DisplayPlayerHandQuadrant(IPlayer player, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.WriteLine($"{player.Name}'s hand:");
            for (int cardIndex = 0; cardIndex < player.Hand.Count; cardIndex++)
            {
                Console.SetCursorPosition(left, top + cardIndex + 1);
                Console.WriteLine($"{cardIndex} : {player.Hand[cardIndex]}");
            }
        }

        private static void DisplayAllPlayersHandQuadrant(IPlayer playerOne, IPlayer playerTwo, IPlayer playerThree, IPlayer playerFour)
        {
            DisplayPlayerHandQuadrant(playerOne, 0, 4);
            DisplayPlayerHandQuadrant(playerTwo, Console.WindowWidth / 2, 4);
            DisplayPlayerHandQuadrant(playerThree, 0, (Console.WindowHeight / 2) + 1);
            DisplayPlayerHandQuadrant(playerFour, Console.WindowWidth / 2, (Console.WindowHeight / 2) + 1);
            Console.WriteLine("\n#########################\n");
        }

        public static void DisplayAllHands(List<Player> players, int dealerIndex)
        {
            DisplayAllPlayersHandQuadrant(players[(dealerIndex) % players.Count],
                                                 players[(dealerIndex + 1) % players.Count],
                                                 players[(dealerIndex + 2) % players.Count],
                                                 players[(dealerIndex + 3) % players.Count]);
        }
    }
}
