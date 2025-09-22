namespace DeuxCentsCardGame.Interfaces
{
    public interface IUIGameView
    {
        void ClearScreen();
        void DisplayMessage(string message);
        void DisplayFormattedMessage(string format, params object[] args);
        void WaitForUser(string message = "Press any key to continue...");
        void DisplayHand(IPlayer player);
        void DisplayAllHands(List<IPlayer> players, int dealerIndex);
    }
}