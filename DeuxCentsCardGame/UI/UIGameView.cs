using DeuxCentsCardGame.Constants;
using DeuxCentsCardGame.Interfaces.UI;
using DeuxCentsCardGame.Interfaces.Models;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Services;

namespace DeuxCentsCardGame.UI
{
    public class UIGameView : IUIGameView
    {
        private readonly IConsoleWrapper _console;
        private readonly CardCollectionHelper _cardHelper;

        public UIGameView(IConsoleWrapper console, CardCollectionHelper cardHelper)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _cardHelper = cardHelper ?? throw new ArgumentNullException(nameof(cardHelper));
        }

        public async Task ClearScreen()
        {
            _console.Clear();
        }

        public async Task DisplayMessage(string message)
        {
            _console.WriteLine(message);
        }

        public async Task DisplayFormattedMessage(string format, params object[] args)
        {
            _console.WriteLine(format, args);
        }

        public async Task<string> GetUserInput(string prompt)
        {
            _console.WriteLine(prompt);
            return _console.ReadLine() ?? string.Empty;
        }

        public async Task GetIntInput(string prompt, int min, int max)
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

        public async Task<string> GetOptionInput(string prompt, string[] options)
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

        public async Task WaitForUser(string message = "Press any key to continue...")
        {
            DisplayMessage(message);
            _console.ReadKey();
        }

        public async Task DisplayHand(IPlayer player)
        {
            _console.WriteLine($"\n{player.Name}'s hand:");
            _console.WriteLine(new string('#', GameConstants.HAND_DISPLAY_SEPARATOR_LENGTH));

            var sortedHand = _cardHelper.SortBySuit(player.Hand);

            for (int cardIndex = 0; cardIndex < player.Hand.Count; cardIndex++)
            {
                int originalIndex = player.Hand.IndexOf(sortedHand[cardIndex]);
                _console.WriteLine($"{originalIndex}: {sortedHand[cardIndex]}");
            }

            _console.WriteLine(new string('#', GameConstants.HAND_DISPLAY_SEPARATOR_LENGTH));
            DisplayHandStatistics(player.Hand);
        }

        public async Task DisplayAllHands(List<IPlayer> players, int dealerIndex)
        {
            _console.WriteLine("\n" + new string('-', GameConstants.ALL_HANDS_SEPARATOR_LENGTH));
            _console.WriteLine("All player hands");
            _console.WriteLine(new string('-', GameConstants.ALL_HANDS_SEPARATOR_LENGTH));

            for (int i = 0; i < players.Count; i++)
            {
                int playerIndex = (dealerIndex + i) % players.Count;
                IPlayer player = players[playerIndex];
                
                string dealerIndicator = playerIndex == dealerIndex ? " (Dealer)" : "";
                _console.WriteLine($"\n{player.Name}{dealerIndicator}:");

                var sortedHand = _cardHelper.SortBySuit(player.Hand);   
                
                for (int cardIndex = 0; cardIndex < player.Hand.Count; cardIndex++)
                {
                    int originalIndex = player.Hand.IndexOf(sortedHand[cardIndex]);
                    _console.WriteLine($"  {originalIndex}: {sortedHand[cardIndex]}");
                }

                DisplayHandStatistics(player.Hand);
            }

            _console.WriteLine("\n" + new string('-', GameConstants.ALL_HANDS_SEPARATOR_LENGTH));
        }

        private void DisplayHandStatistics(List<Card> hand)
        {
            if (hand == null || hand.Count == 0)
                return;

            // Use CardCollectionHelper for statistics
            int totalPoints = _cardHelper.CalculateTotalPoints(hand);
            var suitCounts = _cardHelper.CountBySuit(hand);
            int highCards = _cardHelper.GetHighValueCards(hand, 8).Count;
            
            _console.WriteLine($"  Stats: {totalPoints} pts | High cards: {highCards}");
            
            // Display suit distribution
            var suitDisplay = string.Join(" | ", 
                suitCounts.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            _console.WriteLine($"  Suits: {suitDisplay}");
        }

        public void DisplayHandGroupedBySuit(IPlayer player)
        {
            _console.WriteLine($"\n{player.Name}'s hand (grouped by suit):");
            _console.WriteLine(new string('#', GameConstants.HAND_DISPLAY_SEPARATOR_LENGTH));

            // Use CardCollectionHelper to group by suit
            var grouped = _cardHelper.GroupBySuit(player.Hand);
            
            foreach (var suitGroup in grouped.OrderBy(g => g.Key))
            {
                _console.WriteLine($"\n{suitGroup.Key}:");
                
                // Sort cards within suit by value
                var sortedCards = _cardHelper.SortByValue(suitGroup.Value);
                
                foreach (var card in sortedCards)
                {
                    int originalIndex = player.Hand.IndexOf(card);
                    _console.WriteLine($"  {originalIndex}: {card}");
                }
            }
            
            _console.WriteLine(new string('#', GameConstants.HAND_DISPLAY_SEPARATOR_LENGTH));
        }

        public static void DisplayBettingResults()
        { 
            // should display result here?
        }
    }
}