namespace DeuxCentsCardGame
{
    public interface IUIConsoleGameView
    {
        void ClearScreen();
        void DisplayMessage(string message);
        string GetUserInput(string prompt);
        int GetIntInput(string prompt, int min, int max);
        string GetOptionInput(string prompt, string[] options);
        void WaitForUser(string message = "Press any key to continue...");
        // uncomment once GameState is implemented
        // void ShowGameState(GameState state);
    }
}
