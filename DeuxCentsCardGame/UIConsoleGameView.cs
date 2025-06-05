namespace DeuxCentsCardGame
{
    public class UIConsoleGameView : IUIConsoleGameView
    {
        public void ClearScreen()
        {
            Console.Clear();
            Thread.Sleep(200);
        }

        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
            Thread.Sleep(200);
        }

        public void DisplayFormattedMessage(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Thread.Sleep(200);
        }

        public string GetUserInput(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        public int GetIntInput(string prompt, int min, int max)
        {
            int result;
            bool isValid = false;

            do
            {
                string input = GetUserInput(prompt);
                isValid = int.TryParse(input, out result) && result >= min && result <= max;

                if (!isValid)
                {
                    DisplayMessage($"Invalid input. Please enter a number between {min} and {max}.");
                }
                
            } while(!isValid);

            return result;
        }

        public string GetOptionInput(string prompt, string[] options)
        {
            string result;
            bool isValid = false;

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
            Console.ReadKey();
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
                Thread.Sleep(20);
                Console.WriteLine($"{playerIndex}: {player.Hand[playerIndex]}");
            }
            Thread.Sleep(300);
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
                Thread.Sleep(20);
                Console.SetCursorPosition(left, top + cardIndex + 1);
                Console.WriteLine($"{cardIndex} : {player.Hand[cardIndex]}");
            }
            Thread.Sleep(200);
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
