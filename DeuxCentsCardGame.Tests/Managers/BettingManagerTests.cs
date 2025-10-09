using Moq;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class BettingManagerTests
    {
        private readonly Mock<GameEventManager> _mockEventManager;
        private readonly Mock<IGameConfig> _mockGameConfig;
        private readonly List<Player> _players;
        private readonly BettingManager _bettingManager;

        public BettingManagerTests()
        {
            _mockEventManager = new Mock<GameEventManager>();
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
            _mockGameConfig.Setup(x => x.BetIncrement).Returns(10);
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
                .Returns("60"); // Valid bet

            // Act
            _bettingManager.ExecuteBettingRound();

            // Assert
            _mockEventManager.Verify(x => x.RaiseBettingRoundStarted(It.IsAny<string>()), Times.Once);
            // The bet should be processed without "invalid bet" messages
        }

        [Fact]
        public void ExecuteBettingRound_WithInvalidBet_ShowsError()
        {
            // Arrange
            var invalidBets = new Queue<string>(new[] { "45", "60" }); // 45 is invalid (not multiple of 10)
            _mockEventManager.Setup(x => x.RaiseBetInput(It.IsAny<Player>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(invalidBets.Dequeue);

            // Act
            _bettingManager.ExecuteBettingRound();

            // Assert
            _mockEventManager.Verify(x => x.RaiseInvalidBet(It.IsAny<string>()), Times.AtLeastOnce);
        }
    }
}