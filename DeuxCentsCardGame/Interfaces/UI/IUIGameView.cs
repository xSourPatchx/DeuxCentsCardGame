using DeuxCentsCardGame.Interfaces.Models;

namespace DeuxCentsCardGame.Interfaces.UI
{
    public interface IUIGameView
    {
        void ClearScreen();
        void DisplayMessage(string message);
        void DisplayFormattedMessage(string format, params object[] args);
        string GetUserInput(string prompt);
        int GetIntInput(string prompt, int min, int max);
        string GetOptionInput(string prompt, string[] options);
        void WaitForUser(string message = "Press any key to continue...");
        void DisplayHand(IPlayer player);
        void DisplayAllHands(List<IPlayer> players, int dealerIndex);
    }
}