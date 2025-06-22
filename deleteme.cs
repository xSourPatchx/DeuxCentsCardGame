// Console Implementation
namespace DeuxCentsCardGame
{
    public class UIConsoleGameView : IUIGameView
    {
        private readonly IConsoleWrapper _console;
        
        public UIConsoleGameView() : this(new ConsoleWrapper()) { }
        
        public UIConsoleGameView(IConsoleWrapper console)
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

        public void DisplayPlayerHand(IPlayer player)
        {
            _console.WriteLine($"{player.Name}'s hand:");
            for (int cardIndex = 0; cardIndex < player.Hand.Count; cardIndex++)
            {
                _console.WriteLine($"{cardIndex}: {player.Hand[cardIndex]}");
            }
        }

        public void DisplayAllPlayerHands(List<IPlayer> players, int dealerIndex)
        {
            DisplayAllPlayersHandQuadrant(
                players[dealerIndex % players.Count],
                players[(dealerIndex + 1) % players.Count],
                players[(dealerIndex + 2) % players.Count],
                players[(dealerIndex + 3) % players.Count]
            );
        }

        public void ShowGameState(object gameState)
        {
            // Implementation can be added when GameState class is ready
            DisplayMessage("Game state display not yet implemented");
        }

        private void DisplayAllPlayersHandQuadrant(IPlayer playerOne, IPlayer playerTwo, IPlayer playerThree, IPlayer playerFour)
        {
            DisplayPlayerHandQuadrant(playerOne, 0, 4);
            DisplayPlayerHandQuadrant(playerTwo, _console.WindowWidth / 2, 4);
            DisplayPlayerHandQuadrant(playerThree, 0, (_console.WindowHeight / 2) + 1);
            DisplayPlayerHandQuadrant(playerFour, _console.WindowWidth / 2, (_console.WindowHeight / 2) + 1);
            _console.WriteLine("\n#########################\n");
        }

        private void DisplayPlayerHandQuadrant(IPlayer player, int left, int top)
        {
            _console.SetCursorPosition(left, top);
            _console.WriteLine($"{player.Name}'s hand:");
            for (int cardIndex = 0; cardIndex < player.Hand.Count; cardIndex++)
            {
                _console.SetCursorPosition(left, top + cardIndex + 1);
                _console.WriteLine($"{cardIndex} : {player.Hand[cardIndex]}");
            }
        }
    }
}


// Future Unity Implementation Example
namespace DeuxCentsCardGame.Unity
{
    public class UIUnityGameView : IUIGameView
    {
        // Unity-specific UI components would be injected here
        // private Canvas _gameCanvas;
        // private InputField _inputField;
        // private Text _messageText;
        // etc.

        public void ClearScreen()
        {
            // Clear Unity UI elements
        }

        public void DisplayMessage(string message)
        {
            // Display in Unity UI Text component
        }

        public void DisplayFormattedMessage(string format, params object[] args)
        {
            DisplayMessage(string.Format(format, args));
        }

        public string GetUserInput(string prompt)
        {
            // Unity input handling - likely async
            throw new NotImplementedException("Unity input handling requires async/await pattern");
        }

        public int GetIntInput(string prompt, int min, int max)
        {
            // Unity-specific implementation
            throw new NotImplementedException();
        }

        public string GetOptionInput(string prompt, string[] options)
        {
            // Unity dropdown or button selection
            throw new NotImplementedException();
        }

        public void WaitForUser(string message = "Press any key to continue...")
        {
            // Unity button or touch input
            throw new NotImplementedException();
        }

        public void DisplayPlayerHand(IPlayer player)
        {
            // Display cards in Unity UI
        }

        public void DisplayAllPlayerHands(List<IPlayer> players, int dealerIndex)
        {
            // Unity multiplayer hand display
        }

        public void ShowGameState(object gameState)
        {
            // Unity game state UI
        }
    }
}