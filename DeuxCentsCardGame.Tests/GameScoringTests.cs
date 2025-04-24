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
            // missing stuff here
        }
        
        private Game CreateGameInstance() // Test helpers
        {
            var game = new Game(_mockUI.Object);
            return game; // Create a Game instance with mocked UI
        }
        
        // Left of here
        
        [Fact]
        public void UpdateTeamOnePoints_WhenTeamOneScoreOver100AndTeamOneDidNotBet()
        {
            // Given
            // var game = CreateTestableGame();

        
            // When
        
            // Then
        }

    }
}