using System.Reflection;
using Moq;

namespace DeuxCentsCardGame.Tests
{
    public class GameScoringTests2
    {
        private readonly Mock<IUIConsoleGameView> _mockUI;
        
        public GameScoringTests2()
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

        [Fact]
        public void UpdateTeamPoints_WhenTeamOneScoreOver100AndTeamOneDidNotBet_TeamOneRoundPointsIsZero()
        {
            // 1. Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 100);
            SetPrivateField(game, "_teamTwoTotalPoints", 90);
            SetPrivateField(game, "_teamOneRoundPoints", 60);
            SetPrivateField(game, "_teamTwoRoundPoints", 40);
            SetPrivateField(game, "_winningBid", 50);
            SetPrivateField(game, "_winningBidIndex", 1);

            var hasBet = new bool[4] { false, true, false, false };
            SetPrivateField(game, "_hasBet", hasBet);

            // 2. When or Act - Call the private method using reflection
            var method = typeof(Game).GetMethod("UpdateTeamPoints", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(game, new object[] { true }); // Pass true for Team One

            var teamOneRoundPoints = (int)typeof(Game)
                .GetField("_teamOneRoundPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);

            var teamTwoRoundPoints = (int)typeof(Game)
                .GetField("_teamTwoRoundPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);

            // 3. Then or Assert
            Assert.Equal(0, teamOneRoundPoints);
            Assert.Equal(40, teamTwoRoundPoints); // Team Two round points should remain unchanged
        }

        [Fact]
        public void UpdateTeamPoints_WhenTeamTwoScoreOver100AndTeamTwoDidNotBet_TeamTwoRoundPointsIsZero()
        {
            // 1. Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 90);
            SetPrivateField(game, "_teamTwoTotalPoints", 100);
            SetPrivateField(game, "_teamOneRoundPoints", 60);
            SetPrivateField(game, "_teamTwoRoundPoints", 40);
            SetPrivateField(game, "_winningBid", 50);
            SetPrivateField(game, "_winningBidIndex", 0);

            var hasBet = new bool[4] { true, false, false, false };
            SetPrivateField(game, "_hasBet", hasBet);

            // 2. When or Act - Call the private method using reflection
            var method = typeof(Game).GetMethod("UpdateTeamPoints", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(game, new object[] { false }); // Pass false for Team Two

            var teamOneRoundPoints = (int)typeof(Game)
                .GetField("_teamOneRoundPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);

            var teamTwoRoundPoints = (int)typeof(Game)
                .GetField("_teamTwoRoundPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);

            // 3. Then or Assert
            Assert.Equal(60, teamOneRoundPoints); // Team One round points should remain unchanged
            Assert.Equal(0, teamTwoRoundPoints);
        }
        
        [Fact]
        public void UpdateTeamPoints_WhenTeamOneScoreUnder100AndMadeBid_TeamOneRoundPointsIncreases()
        {
            // 1. Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 50);
            SetPrivateField(game, "_teamTwoTotalPoints", 60);
            SetPrivateField(game, "_teamOneRoundPoints", 60);
            SetPrivateField(game, "_teamTwoRoundPoints", 40);
            SetPrivateField(game, "_winningBid", 50);
            SetPrivateField(game, "_winningBidIndex", 0);

            var hasBet = new bool[4] { true, false, false, false };
            SetPrivateField(game, "_hasBet", hasBet);

            int initialTeamOneTotalPoints = 50;

            // 2. When or Act - Call the private method using reflection
            var method = typeof(Game).GetMethod("UpdateTeamPoints", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(game, new object[] { true }); // Pass true for Team One

            var teamOneTotalPoints = (int)typeof(Game)
                .GetField("_teamOneTotalPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);

            // 3. Then or Assert
            Assert.Equal(initialTeamOneTotalPoints + 60, teamOneTotalPoints);
        }
        
        [Fact]
        public void UpdateTeamPoints_WhenTeamOneScoreUnder100AndFailedBid_TeamOneRoundPointsDecrease()
        {
            // 1. Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 50);
            SetPrivateField(game, "_teamTwoTotalPoints", 60);
            SetPrivateField(game, "_teamOneRoundPoints", 40); // Less than winning bid
            SetPrivateField(game, "_teamTwoRoundPoints", 60);
            SetPrivateField(game, "_winningBid", 50);
            SetPrivateField(game, "_winningBidIndex", 0);

            var hasBet = new bool[4] { true, false, false, false };
            SetPrivateField(game, "_hasBet", hasBet);

            int initialTeamOneTotalPoints = 50;

            // 2. When or Act - Call the private method using reflection
            var method = typeof(Game).GetMethod("UpdateTeamPoints", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(game, new object[] { true }); // Pass true for Team One

            var teamOneTotalPoints = (int)typeof(Game)
                .GetField("_teamOneTotalPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);

            // 3. Then or Assert
            Assert.Equal(initialTeamOneTotalPoints - 50, teamOneTotalPoints); // Should lose the bid amount
        }
    }
}