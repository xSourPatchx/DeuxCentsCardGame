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

        public void DisplayHand(IPlayer player)
        {
            _console.WriteLine($"\n{player.Name}'s hand:");
            _console.WriteLine(new string('#', 40));

            for (int cardIndex = 0; cardIndex < player.Hand.Count; cardIndex++)
            {
                _console.WriteLine($"{cardIndex}: {player.Hand[cardIndex]}");
            }
            _console.WriteLine(new string('#', 40));
        }

        public void DisplayAllHands(List<IPlayer> players, int dealerIndex)
        {
            _console.WriteLine("\n" + new string('-', 60));
            _console.WriteLine("All player hands");
            _console.WriteLine(new string('-', 60));

            for (int i = 0; i < players.Count; i++)
            {
                int playerIndex = (dealerIndex + i) % players.Count;
                IPlayer player = players[playerIndex];
                
                string dealerIndicator = playerIndex == dealerIndex ? " (Dealer)" : "";
                _console.WriteLine($"\n{player.Name}{dealerIndicator}:");
                
                for (int cardIndex = 0; cardIndex < player.Hand.Count; cardIndex++)
                {
                    _console.WriteLine($"  {cardIndex}: {player.Hand[cardIndex]}");
                }
            }

            _console.WriteLine("\n" + new string('-', 60));
        }

        public static void DisplayBettingResults()
        { 
            // should display result here?
        }
    }
}