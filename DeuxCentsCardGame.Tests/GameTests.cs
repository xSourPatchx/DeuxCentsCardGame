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
            _mockUI = new Mock<IUIConsoleGameView>();
        }

        private Game CreateGameInstance()
        {
            // Create a Game instance with mocked UI
            return new Game(_mockUI.Object);
        }
    }
}