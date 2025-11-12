using DeuxCentsCardGame.Interfaces.Managers;

namespace DeuxCentsCardGame.Managers
{
    public class PlayerTurnManager : IPlayerTurnManager
    {
        private readonly int _totalPlayers;
        public int CurrentPlayerIndex { get; private set; }
        public int StartingPlayerIndex { get; private set; }
        public bool IsTurnActive { get; private set; }

        public PlayerTurnManager(int totalPlayers)
        {
            if (totalPlayers < 2)
                throw new ArgumentException("Total players must be at least 2", nameof(totalPlayers));
                
            _totalPlayers = totalPlayers;
            CurrentPlayerIndex = -1;
            StartingPlayerIndex = -1;
            IsTurnActive = false;
        }

        public void InitializeTurnSequence(int startingPlayerIndex)
        {
            ValidatePlayerIndex(startingPlayerIndex);
            
            StartingPlayerIndex = startingPlayerIndex;
            CurrentPlayerIndex = startingPlayerIndex;
            IsTurnActive = true;
        }

        public int AdvanceToNextPlayer()
        {
            if (!IsTurnActive)
                throw new InvalidOperationException("Turn sequence is not active. Call InitializeTurnSequence first.");
                
            CurrentPlayerIndex = CalculateNextPlayerIndex(CurrentPlayerIndex);
            return CurrentPlayerIndex;
        }

        public int PeekNextPlayer()
        {
            if (!IsTurnActive)
                throw new InvalidOperationException("Turn sequence is not active. Call InitializeTurnSequence first.");
                
            return CalculateNextPlayerIndex(CurrentPlayerIndex);
        }

        public void SetCurrentPlayer(int playerIndex)
        {
            ValidatePlayerIndex(playerIndex);
            CurrentPlayerIndex = playerIndex;
            IsTurnActive = true;
        }

        public void ResetTurnSequence()
        {
            CurrentPlayerIndex = -1;
            StartingPlayerIndex = -1;
            IsTurnActive = false;
        }

        public int GetTurnOrderPosition(int playerIndex)
        {
            if (!IsTurnActive)
                throw new InvalidOperationException("Turn sequence is not active.");
                
            ValidatePlayerIndex(playerIndex);
            
            // Calculate distance from current player in circular order
            int position = (playerIndex - CurrentPlayerIndex + _totalPlayers) % _totalPlayers;
            return position;
        }

        public List<int> GetTurnOrder()
        {
            if (!IsTurnActive)
                throw new InvalidOperationException("Turn sequence is not active.");

            var turnOrder = new List<int>(_totalPlayers);

            for (int i = 0; i < _totalPlayers; i++)
            {
                int playerIndex = (CurrentPlayerIndex + i) % _totalPlayers;
                turnOrder.Add(playerIndex);
            }

            return turnOrder;
        }

        public int RotateDealer(int currentDealerIndex)
        {
            ValidatePlayerIndex(currentDealerIndex);
            return CalculateNextPlayerIndex(currentDealerIndex);
        }

        public int GetPlayerLeftOfDealer(int dealerIndex)
        {
            ValidatePlayerIndex(dealerIndex);
            return CalculateNextPlayerIndex(dealerIndex);
        }
        
        public int GetPlayerRightOfDealer(int dealerIndex)
        {
            ValidatePlayerIndex(dealerIndex);
            return (dealerIndex - 1 + _totalPlayers) % _totalPlayers;
        }

        private int CalculateNextPlayerIndex(int currentIndex)
        {
            return (currentIndex + 1) % _totalPlayers;
        }

        private void ValidatePlayerIndex(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= _totalPlayers)
                throw new ArgumentOutOfRangeException(nameof(playerIndex), 
                    $"Player index must be between 0 and {_totalPlayers - 1}");
        }
    }
}