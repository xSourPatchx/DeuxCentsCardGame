using System.Reflection;
using Moq;

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
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                throw new ArgumentException($"Field {fieldName} not found");
            }
        }

        [Fact]
        public void UpdateTeamOnePoints_WhenTeamOneScoreOver100AndTeamOneDidNotBet_TeamOneRoundPointsIsZero()
        {
            // 1. Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 100);
            SetPrivateField(game, "_teamOneRoundPoints", 60);
            SetPrivateField(game, "_winningBid", 50);
            SetPrivateField(game, "_winningBidIndex", 0); // team one won the bet

            var hasBet = new bool[4] { false, false, false, false }; // team two hasnt placed a bet
            SetPrivateField(game, "_hasBet", hasBet);

            // 2. When or Act - Call the private method using reflection
            var method = typeof(Game).GetMethod("UpdateTeamPoints", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(game, new object[] { true });

            var teamOneRoundPoints = (int)typeof(Game)
                .GetField("_teamOneRoundPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);

            // 3. Then or Assert
            Assert.Equal(0, teamOneRoundPoints);
        }
    }
}