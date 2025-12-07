using DeuxCentsCardGame.Interfaces.Models;

namespace DeuxCentsCardGame.Interfaces.UI
{
    public interface IUIGameView
    {
        Task ClearScreen();
        Task DisplayMessage(string message);
        Task DisplayFormattedMessage(string format, params object[] args);
        Task<string> GetUserInput(string prompt);
        Task<int> GetIntInput(string prompt, int min, int max);
        Task<string> GetOptionInput(string prompt, string[] options);
        Task WaitForUser(string message = "Press any key to continue...");
        Task DisplayHand(IPlayer player);
        Task DisplayAllHands(List<IPlayer> players, int dealerIndex);
    }
}