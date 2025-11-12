namespace DeuxCentsCardGame.Interfaces.Managers
{
    public interface IPlayerTurnManager
    {
        // Gets the index of the current player whose turn it is
        int CurrentPlayerIndex { get; }

        // Gets the index of the starting player for the current phase
        int StartingPlayerIndex { get; }
        
        // Gets whether turn management is currently active
        bool IsTurnActive { get; }
        
        // Initializes a new turn sequence starting with the specified player
        void InitializeTurnSequence(int startingPlayerIndex);
        
        // Advances to the next player in turn order
        int AdvanceToNextPlayer();
        
        // Gets the next player index without advancing the turn
        int PeekNextPlayer();
        
        // Sets the current player to a specific index
        void SetCurrentPlayer(int playerIndex);
        
        // Resets the turn manager to its initial state
        void ResetTurnSequence();
        
        // Gets the relative position of a player in the current turn order
        int GetTurnOrderPosition(int playerIndex);

        // Gets all players in current turn order
        List<int> GetTurnOrder();

        // Rotates to the next dealer index
        int RotateDealer(int currentDealerIndex);

        // Calculates the turn order starting from a specific player
        int GetPlayerLeftOfDealer(int dealerIndex);
        
        // Calculates the turn order starting from a specific player
        int GetPlayerRightOfDealer(int dealerIndex);
    }
}