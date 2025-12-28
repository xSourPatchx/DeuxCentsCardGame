using DeuxCentsCardGame.Managers;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class PlayerTurnManagerTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidPlayerCount_CreatesManager()
        {
            // Arrange & Act
            var turnManager = new PlayerTurnManager(4);

            // Assert
            Assert.NotNull(turnManager);
            Assert.Equal(-1, turnManager.CurrentPlayerIndex);
            Assert.Equal(-1, turnManager.StartingPlayerIndex);
            Assert.False(turnManager.IsTurnActive);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        public void Constructor_WithInvalidPlayerCount_ThrowsException(int playerCount)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new PlayerTurnManager(playerCount));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(8)]
        public void Constructor_WithVariousValidPlayerCounts_Succeeds(int playerCount)
        {
            // Act
            var turnManager = new PlayerTurnManager(playerCount);

            // Assert
            Assert.NotNull(turnManager);
            Assert.False(turnManager.IsTurnActive);
        }

        #endregion

        #region InitializeTurnSequence Tests

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void InitializeTurnSequence_WithValidIndex_SetsCorrectStartingPlayer(int startingIndex)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act
            turnManager.InitializeTurnSequence(startingIndex);

            // Assert
            Assert.Equal(startingIndex, turnManager.StartingPlayerIndex);
            Assert.Equal(startingIndex, turnManager.CurrentPlayerIndex);
            Assert.True(turnManager.IsTurnActive);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        [InlineData(5)]
        public void InitializeTurnSequence_WithInvalidIndex_ThrowsException(int startingIndex)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                turnManager.InitializeTurnSequence(startingIndex));
        }

        #endregion

        #region AdvanceToNextPlayer Tests

        [Fact]
        public void AdvanceToNextPlayer_WhenNotInitialized_ThrowsException()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                turnManager.AdvanceToNextPlayer());
        }

        [Fact]
        public void AdvanceToNextPlayer_AdvancesSequentially()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(0);

            // Act & Assert
            Assert.Equal(1, turnManager.AdvanceToNextPlayer());
            Assert.Equal(2, turnManager.AdvanceToNextPlayer());
            Assert.Equal(3, turnManager.AdvanceToNextPlayer());
        }

        [Fact]
        public void AdvanceToNextPlayer_WrapsAroundToZero()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(3);

            // Act
            int nextPlayer = turnManager.AdvanceToNextPlayer();

            // Assert
            Assert.Equal(0, nextPlayer);
            Assert.Equal(0, turnManager.CurrentPlayerIndex);
        }

        [Fact]
        public void AdvanceToNextPlayer_UpdatesCurrentPlayerIndex()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(1);

            // Act
            turnManager.AdvanceToNextPlayer();

            // Assert
            Assert.Equal(2, turnManager.CurrentPlayerIndex);
        }

        #endregion

        #region PeekNextPlayer Tests

        [Fact]
        public void PeekNextPlayer_WhenNotInitialized_ThrowsException()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                turnManager.PeekNextPlayer());
        }

        [Fact]
        public void PeekNextPlayer_ReturnsNextPlayerWithoutAdvancing()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(1);

            // Act
            int peekedPlayer = turnManager.PeekNextPlayer();

            // Assert
            Assert.Equal(2, peekedPlayer);
            Assert.Equal(1, turnManager.CurrentPlayerIndex); // Should not change
        }

        [Fact]
        public void PeekNextPlayer_WrapsAround()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(3);

            // Act
            int peekedPlayer = turnManager.PeekNextPlayer();

            // Assert
            Assert.Equal(0, peekedPlayer);
            Assert.Equal(3, turnManager.CurrentPlayerIndex); // Should not change
        }

        [Fact]
        public void PeekNextPlayer_CalledMultipleTimes_ReturnsSameValue()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(0);

            // Act
            int peek1 = turnManager.PeekNextPlayer();
            int peek2 = turnManager.PeekNextPlayer();
            int peek3 = turnManager.PeekNextPlayer();

            // Assert
            Assert.Equal(peek1, peek2);
            Assert.Equal(peek2, peek3);
            Assert.Equal(0, turnManager.CurrentPlayerIndex);
        }

        #endregion

        #region SetCurrentPlayer Tests

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SetCurrentPlayer_WithValidIndex_SetsPlayerAndActivatesTurn(int playerIndex)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act
            turnManager.SetCurrentPlayer(playerIndex);

            // Assert
            Assert.Equal(playerIndex, turnManager.CurrentPlayerIndex);
            Assert.True(turnManager.IsTurnActive);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        [InlineData(10)]
        public void SetCurrentPlayer_WithInvalidIndex_ThrowsException(int playerIndex)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                turnManager.SetCurrentPlayer(playerIndex));
        }

        [Fact]
        public void SetCurrentPlayer_CanChangeCurrentPlayerMidSequence()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(0);
            turnManager.AdvanceToNextPlayer(); // Now at player 1

            // Act
            turnManager.SetCurrentPlayer(3);

            // Assert
            Assert.Equal(3, turnManager.CurrentPlayerIndex);
            Assert.Equal(0, turnManager.AdvanceToNextPlayer()); // Should wrap from 3 to 0
        }

        #endregion

        #region ResetTurnSequence Tests

        [Fact]
        public void ResetTurnSequence_ResetsAllProperties()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(2);
            turnManager.AdvanceToNextPlayer();

            // Act
            turnManager.ResetTurnSequence();

            // Assert
            Assert.Equal(-1, turnManager.CurrentPlayerIndex);
            Assert.Equal(-1, turnManager.StartingPlayerIndex);
            Assert.False(turnManager.IsTurnActive);
        }

        [Fact]
        public void ResetTurnSequence_CanBeCalledMultipleTimes()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(0);

            // Act
            turnManager.ResetTurnSequence();
            turnManager.ResetTurnSequence();
            turnManager.ResetTurnSequence();

            // Assert
            Assert.False(turnManager.IsTurnActive);
            Assert.Equal(-1, turnManager.CurrentPlayerIndex);
        }

        #endregion

        #region GetTurnOrderPosition Tests

        [Fact]
        public void GetTurnOrderPosition_WhenNotActive_ThrowsException()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                turnManager.GetTurnOrderPosition(0));
        }

        [Theory]
        [InlineData(0, 0, 0)] // Current player, distance 0
        [InlineData(0, 1, 1)] // Next player, distance 1
        [InlineData(0, 2, 2)] // Two away, distance 2
        [InlineData(0, 3, 3)] // Three away, distance 3
        public void GetTurnOrderPosition_ReturnsCorrectDistance(
            int currentPlayer, int queryPlayer, int expectedDistance)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(currentPlayer);

            // Act
            int position = turnManager.GetTurnOrderPosition(queryPlayer);

            // Assert
            Assert.Equal(expectedDistance, position);
        }

        [Fact]
        public void GetTurnOrderPosition_WrapsAroundCorrectly()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(2); // Start at player 2

            // Act & Assert
            Assert.Equal(0, turnManager.GetTurnOrderPosition(2)); // Current
            Assert.Equal(1, turnManager.GetTurnOrderPosition(3)); // Next
            Assert.Equal(2, turnManager.GetTurnOrderPosition(0)); // Wraps to 0
            Assert.Equal(3, turnManager.GetTurnOrderPosition(1)); // Wraps to 1
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public void GetTurnOrderPosition_WithInvalidIndex_ThrowsException(int playerIndex)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(0);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                turnManager.GetTurnOrderPosition(playerIndex));
        }

        #endregion

        #region GetTurnOrder Tests

        [Fact]
        public void GetTurnOrder_WhenNotActive_ThrowsException()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                turnManager.GetTurnOrder());
        }

        [Fact]
        public void GetTurnOrder_ReturnsCorrectSequence()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(0);

            // Act
            var turnOrder = turnManager.GetTurnOrder();

            // Assert
            Assert.Equal(4, turnOrder.Count);
            Assert.Equal(new[] { 0, 1, 2, 3 }, turnOrder);
        }

        [Fact]
        public void GetTurnOrder_StartsFromCurrentPlayer()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(2);

            // Act
            var turnOrder = turnManager.GetTurnOrder();

            // Assert
            Assert.Equal(new[] { 2, 3, 0, 1 }, turnOrder);
        }

        [Fact]
        public void GetTurnOrder_UpdatesAfterAdvancing()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(0);
            turnManager.AdvanceToNextPlayer(); // Now at player 1

            // Act
            var turnOrder = turnManager.GetTurnOrder();

            // Assert
            Assert.Equal(new[] { 1, 2, 3, 0 }, turnOrder);
        }

        #endregion

        #region RotateDealer Tests

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 0)]
        public void RotateDealer_ReturnsNextDealer(int currentDealer, int expectedNextDealer)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act
            int nextDealer = turnManager.RotateDealer(currentDealer);

            // Assert
            Assert.Equal(expectedNextDealer, nextDealer);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public void RotateDealer_WithInvalidIndex_ThrowsException(int dealerIndex)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                turnManager.RotateDealer(dealerIndex));
        }

        [Fact]
        public void RotateDealer_CanBeCalledWithoutActiveTurn()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act
            int nextDealer = turnManager.RotateDealer(2);

            // Assert
            Assert.Equal(3, nextDealer);
            Assert.False(turnManager.IsTurnActive); // Should not activate turn
        }

        #endregion

        #region GetPlayerLeftOfDealer Tests

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 0)]
        public void GetPlayerLeftOfDealer_ReturnsCorrectPlayer(int dealer, int expectedLeft)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act
            int leftPlayer = turnManager.GetPlayerLeftOfDealer(dealer);

            // Assert
            Assert.Equal(expectedLeft, leftPlayer);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public void GetPlayerLeftOfDealer_WithInvalidIndex_ThrowsException(int dealerIndex)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                turnManager.GetPlayerLeftOfDealer(dealerIndex));
        }

        #endregion

        #region GetPlayerRightOfDealer Tests

        [Theory]
        [InlineData(0, 3)]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        public void GetPlayerRightOfDealer_ReturnsCorrectPlayer(int dealer, int expectedRight)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act
            int rightPlayer = turnManager.GetPlayerRightOfDealer(dealer);

            // Assert
            Assert.Equal(expectedRight, rightPlayer);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public void GetPlayerRightOfDealer_WithInvalidIndex_ThrowsException(int dealerIndex)
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                turnManager.GetPlayerRightOfDealer(dealerIndex));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void PlayerTurnManager_CompleteRoundScenario()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            int dealerIndex = 0;

            // Act - Simulate a complete round
            // 1. Initialize turn sequence (player left of dealer starts)
            int startingPlayer = turnManager.GetPlayerLeftOfDealer(dealerIndex);
            turnManager.InitializeTurnSequence(startingPlayer);

            // 2. Each player takes their turn
            var playersWhoPlayed = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                playersWhoPlayed.Add(turnManager.CurrentPlayerIndex);
                if (i < 3) // Don't advance after last player
                    turnManager.AdvanceToNextPlayer();
            }

            // 3. Rotate dealer for next round
            int nextDealer = turnManager.RotateDealer(dealerIndex);

            // 4. Reset for next round
            turnManager.ResetTurnSequence();

            // Assert
            Assert.Equal(new[] { 1, 2, 3, 0 }, playersWhoPlayed);
            Assert.Equal(1, nextDealer);
            Assert.False(turnManager.IsTurnActive);
        }

        [Fact]
        public void PlayerTurnManager_MultipleRoundsScenario()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            int dealerIndex = 0;

            // Act - Simulate three rounds
            for (int round = 0; round < 3; round++)
            {
                int startingPlayer = turnManager.GetPlayerLeftOfDealer(dealerIndex);
                turnManager.InitializeTurnSequence(startingPlayer);

                // Play through the round
                for (int i = 0; i < 3; i++)
                {
                    turnManager.AdvanceToNextPlayer();
                }

                dealerIndex = turnManager.RotateDealer(dealerIndex);
                turnManager.ResetTurnSequence();
            }

            // Assert - After 3 rounds starting from dealer 0, dealer should be at index 3
            Assert.Equal(3, dealerIndex);
        }

        [Fact]
        public void PlayerTurnManager_PeekAndAdvanceInteraction()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            turnManager.InitializeTurnSequence(0);

            // Act
            int peeked1 = turnManager.PeekNextPlayer(); // Should be 1
            int advanced1 = turnManager.AdvanceToNextPlayer(); // Should be 1
            int peeked2 = turnManager.PeekNextPlayer(); // Should be 2
            int advanced2 = turnManager.AdvanceToNextPlayer(); // Should be 2

            // Assert
            Assert.Equal(1, peeked1);
            Assert.Equal(1, advanced1);
            Assert.Equal(2, peeked2);
            Assert.Equal(2, advanced2);
        }

        [Fact]
        public void PlayerTurnManager_DealerPositionHelpers()
        {
            // Arrange
            var turnManager = new PlayerTurnManager(4);
            int dealerIndex = 1;

            // Act
            int leftOfDealer = turnManager.GetPlayerLeftOfDealer(dealerIndex);
            int rightOfDealer = turnManager.GetPlayerRightOfDealer(dealerIndex);

            // Assert
            Assert.Equal(2, leftOfDealer);  // Player to the left
            Assert.Equal(0, rightOfDealer); // Player to the right
        }

        #endregion
    }
}