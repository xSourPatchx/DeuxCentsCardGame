namespace DeuxCentsCardGame.Interfaces
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
        // void DisplayPlayerHand(IPlayer player);
        // void DisplayAllPlayerHands(List<IPlayer> players, int dealerIndex);

        // Optional for future Unity implementation
        // void ShowGameState(GameState state);// or void ShowGameState(object gameState);
    }
}