using DeuxCentsCardGame.Interfaces.Models;
using DeuxCentsCardGame.Interfaces.UI;

namespace DeuxCentsCardGame.UI
{
    public class UIGameView : IUIGameView
    {
        private readonly IConsoleWrapper _console;

        public UIGameView(IConsoleWrapper console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task ClearScreen()
        {
            _console.Clear();
            await Task.CompletedTask;
        }

        public async Task DisplayMessage(string message)
        {
            _console.WriteLine(message);
            await Task.CompletedTask;
        }

        public async Task DisplayFormattedMessage(string format, params object[] args)
        {
            _console.WriteLine(format, args);
            await Task.CompletedTask;
        }

        public async Task<string> GetUserInput(string prompt)
        {
            _console.WriteLine(prompt);
            string? input = _console.ReadLine();
            await Task.CompletedTask;
            return input ?? string.Empty;
        }

        public async Task<int> GetIntInput(string prompt, int min, int max)
        {
            while (true)
            {
                _console.WriteLine(prompt);
                string? input = _console.ReadLine();

                if (int.TryParse(input, out int result) && result >= min && result <= max)
                {
                    await Task.CompletedTask;
                    return result;
                }

                _console.WriteLine($"Invalid input. Please enter a number between {min} and {max}.");
            }
        }

        public async Task<string> GetOptionInput(string prompt, string[] validOptions)
        {
            while (true)
            {
                _console.WriteLine(prompt);
                string? input = _console.ReadLine()?.ToLower();

                if (input != null && validOptions.Contains(input.ToLower()))
                {
                    await Task.CompletedTask;
                    return input;
                }

                _console.WriteLine($"Invalid input. Valid options: {string.Join(", ", validOptions)}");
            }
        }

        public async Task WaitForUser(string message = "Press any key to continue...")
        {
            _console.WriteLine(message);
            _console.ReadKey();
            await Task.CompletedTask;
        }

        public async Task DisplayHand(IPlayer player)
        {
            _console.WriteLine($"\n{player.Name}'s hand:");
            for (int i = 0; i < player.Hand.Count; i++)
            {
                _console.WriteLine($"  [{i}] {player.Hand[i]}");
            }
            await Task.CompletedTask;
        }

        public async Task DisplayAllHands(List<IPlayer> players, int currentPlayerIndex)
        {
            _console.WriteLine("\nAll players' hands:");
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                string dealerMarker = i == currentPlayerIndex ? " (Dealer)" : "";
                _console.WriteLine($"\n{player.Name}{dealerMarker}:");
                
                for (int j = 0; j < player.Hand.Count; j++)
                {
                    _console.WriteLine($"  [{j}] {player.Hand[j]}");
                }
            }
            await Task.CompletedTask;
        }

        public static void DisplayBettingResults()
        { 
            // could display result here?
        }
    }
}