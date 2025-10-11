using Moq;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class BettingManagerTests
    {
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly Mock<IGameConfig> _mockGameConfig;
        private readonly List<Player> _players;
        private readonly BettingManager _bettingManager;

        public BettingManagerTests()
        {
            _mockEventManager = new Mock<IGameEventManager>();
            _mockGameConfig = new Mock<IGameConfig>();
            
            _players = new List<Player>
            {
                new Player("Player1"),
                new Player("Player2"),
                new Player("Player3"),
                new Player("Player4")
            };

            _mockGameConfig.Setup(x => x.MinimumBet).Returns(50);
            _mockGameConfig.Setup(x => x.MaximumBet).Returns(100);
            _mockGameConfig.Setup(x => x.BetIncrement).Returns(5);
            _mockGameConfig.Setup(x => x.MinimumPlayersToPass).Returns(3);

            _bettingManager = new BettingManager(_players, 0, _mockEventManager.Object, _mockGameConfig.Object);
        }

        [Fact]
        public void ResetBettingRound_ResetsAllPlayersAndState()
        {
            // Arrange
            _players[0].CurrentBid = 50;
            _players[0].HasPassed = true;
            _bettingManager.CurrentWinningBid = 50;

            // Act
            _bettingManager.ResetBettingRound();

            // Assert
            Assert.Equal(0, _bettingManager.CurrentWinningBid);
            Assert.False(_bettingManager.IsBettingRoundComplete);
            Assert.All(_players, player => Assert.False(player.HasPassed));
        }

        [Fact]
        public void ExecuteBettingRound_WithValidBet_ProcessesCorrectly()
        {
            // Arrange
            _mockEventManager.Setup(x => x.RaiseBetInput(It.IsAny<Player>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns("60"); // Valid bet (multiple of 5)

            // Act
            _bettingManager.ExecuteBettingRound();

            // Assert
            _mockEventManager.Verify(x => x.RaiseBettingRoundStarted(It.IsAny<string>()), Times.Once);
            Assert.True(_bettingManager.IsBettingRoundComplete);
        }

        [Fact]
        public void ExecuteBettingRound_WithInvalidBet_ShowsError()
        {
            // Arrange
            var invalidBets = new Queue<string>(new[] { "47", "60" }); // 47 is invalid (not multiple of 5)
            _mockEventManager.Setup(x => x.RaiseBetInput(It.IsAny<Player>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(invalidBets.Dequeue);

            // Act
            _bettingManager.ExecuteBettingRound();

            // Assert
            _mockEventManager.Verify(x => x.RaiseInvalidBet(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public void ExecuteBettingRound_WithMaxBet_EndsImmediately()
        {
            // Arrange
            _mockEventManager.Setup(x => x.RaiseBetInput(It.IsAny<Player>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns("100");

            // Act
            _bettingManager.ExecuteBettingRound();

            // Assert
            Assert.True(_bettingManager.IsBettingRoundComplete);
            Assert.Equal(100, _bettingManager.CurrentWinningBid);
            _mockEventManager.Verify(x => x.RaiseBettingAction(It.IsAny<Player>(), It.IsAny<int>(), true, It.IsAny<bool>()), Times.AtLeast(3));
        }

        [Fact]
        public void ExecuteBettingRound_ThreePassesForcesLastPlayerToBet50()
        {
            // Arrange
            var responses = new Queue<string>(new[] { "pass", "pass", "pass", "pass" });
            _mockEventManager.Setup(x => x.RaiseBetInput(It.IsAny<Player>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(() => responses.Count > 0 ? responses.Dequeue() : "pass");

            // Act
            _bettingManager.ExecuteBettingRound();

            // Assert
            Assert.Equal(50, _bettingManager.CurrentWinningBid);
            Assert.True(_bettingManager.IsBettingRoundComplete);
        }

        [Fact]
        public void ExecuteBettingRound_FirstThreePasses_LastPlayerForcedToBet50()
        {
            // Arrange
            int callCount = 0;
            _mockEventManager.Setup(x => x.RaiseBetInput(It.IsAny<Player>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(() =>
                {
                    callCount++;
                    // First three players pass, fourth should be forced to bet 50
                    return callCount <= 3 ? "pass" : "pass"; // The fourth will be handled by the forced bet logic
                });

            // Act
            _bettingManager.ExecuteBettingRound();

            // Assert
            Assert.Equal(50, _bettingManager.CurrentWinningBid);
            Assert.True(_bettingManager.IsBettingRoundComplete);
        }
    }
}