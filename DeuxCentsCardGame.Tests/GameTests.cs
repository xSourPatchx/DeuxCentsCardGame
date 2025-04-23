using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using DeuxCentsCardGame;

namespace DeuxCentsCardGame.Tests
{
    public class GameTests
    {
        private readonly Mock<IUIConsoleGameView> _mockUI;
        
        public GameTests()
        {
            // Set up mock UI for testing
            _mockUI = new Mock<IUIConsoleGameView>();
        }
        
        // Test helpers
        private Game CreateGameInstance()
        {
            // Create a Game instance with mocked UI
            return new Game(_mockUI.Object);
        }
        
        // Place your tests here
    }
}