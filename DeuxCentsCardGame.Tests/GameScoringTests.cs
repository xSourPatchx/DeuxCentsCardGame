// using System;
// using System.Collections.Generic;
// using Xunit;
using Moq;
// using DeuxCentsCardGame;

namespace DeuxCentsCardGame.Tests
{
    public class GameScoringTests
    {
        private readonly Mock<IUIConsoleGameView> _mockUI;
        
        public GameScoringTests()
        {
            _mockUI = new Mock<IUIConsoleGameView>(); // Set up mock UI for testing
            // Tell the mock UI object to accept any string message without actually displaying it
            // and mark this setup as something we can verify happened later if needed
            _mockUI.Setup(ui => ui.DisplayMessage(It.IsAny<string>())).Verifiable();

            // Tell the mock UI object to accept any formatted string message with any array of parameters
            // without actually displaying it, and mark this setup as something we can verify happened later if needed
            _mockUI.Setup(ui => ui.DisplayFormattedMessage(
                    It.IsAny<string>(),  // Accept any format string
                    It.IsAny<object[]>() // Accept any array of parameters
                )).Verifiable();
        }
        
        private Game CreateGameInstance() // Test helpers
        {
            var game = new Game(_mockUI.Object);
            return game; // Create a Game instance with mocked UI
        }
        
        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                throw new ArgumentException($"Field {fieldName} not found in {obj.GetType().FullName}");
            }
        }


        [Fact]
        public void UpdateTeamOnePoints_WhenTeamOneScoreOver100AndTeamOneDidNotBet()
        {
            // 1. Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 50);
            SetPrivateField(game, "_teamTwoTotalPoints", 120);
            SetPrivateField(game, "_teamOneRoundPoints", 50);
            SetPrivateField(game, "_teamTwoRoundPoints", 50);
            SetPrivateField(game, "_winningBid", 50);
            SetPrivateField(game, "_winningBidIndex", 1);

            var _hasBet = new bool[4] {true, false, false, false};
            SetPrivateField(game, "_hasBet", _hasBet);
            
            // left off here

            // 2. When or Act

            // 3. Then or Assert
        }
    }
}