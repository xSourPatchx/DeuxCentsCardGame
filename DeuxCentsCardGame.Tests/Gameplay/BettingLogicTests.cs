using DeuxCentsCardGame.Gameplay;
using DeuxCentsCardGame.Models;
using Moq;

namespace DeuxCentsCardGame.Tests.Gameplay
{
    public class BettingLogicTests
    {
        private readonly BettingLogic _bettingLogic;
        private readonly Mock<Interfaces.GameConfig.IGameConfig> _mockGameConfig;

        public BettingLogicTests()
        {
            _mockGameConfig = new Mock<Interfaces.GameConfig.IGameConfig>();
            _mockGameConfig.Setup(c => c.MinimumBet).Returns(50);
            _mockGameConfig.Setup(c => c.MaximumBet).Returns(100);
            _mockGameConfig.Setup(c => c.PassedBidValue).Returns(-1);
            
            _bettingLogic = new BettingLogic(_mockGameConfig.Object);
        }

        [Fact]
        public void MarkPlayerAsPassed_SetsHasPassedToTrue()
        {
            // Arrange
            var player = new Player("Test Player");

            // Act
            _bettingLogic.MarkPlayerAsPassed(player);

            // Assert
            Assert.True(player.HasPassed);
        }

        [Fact]
        public void MarkPlayerAsPassed_SetsCurrentBidToNegativeOne_WhenPlayerHasNotBet()
        {
            // Arrange
            var player = new Player("Test Player");

            // Act
            _bettingLogic.MarkPlayerAsPassed(player);

            // Assert
            Assert.Equal(-1, player.CurrentBid);
        }

        [Fact]
        public void MarkPlayerAsPassed_DoesNotChangeBid_WhenPlayerHasBet()
        {
            // Arrange
            var player = new Player("Test Player");
            player.HasBet = true;
            player.CurrentBid = 75;

            // Act
            _bettingLogic.MarkPlayerAsPassed(player);

            // Assert
            Assert.Equal(75, player.CurrentBid);
            Assert.True(player.HasPassed);
        }

        [Fact]
        public void RecordPlayerBet_SetsBetAmount()
        {
            // Arrange
            var player = new Player("Test Player");

            // Act
            _bettingLogic.RecordPlayerBet(player, 65);

            // Assert
            Assert.Equal(65, player.CurrentBid);
            Assert.True(player.HasBet);
        }

        [Fact]
        public void ForceOtherPlayersToPass_PassesAllExceptSpecifiedPlayer()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Player 1"),
                new Player("Player 2"),
                new Player("Player 3"),
                new Player("Player 4")
            };

            // Act
            _bettingLogic.ForceOtherPlayersToPass(players, 1);

            // Assert
            Assert.True(players[0].HasPassed);
            Assert.False(players[1].HasPassed); // This player not forced to pass
            Assert.True(players[2].HasPassed);
            Assert.True(players[3].HasPassed);
        }

        [Fact]
        public void ForceOtherPlayersToPass_DoesNotChangeAlreadyPassedPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Player 1") { HasPassed = true },
                new Player("Player 2"),
                new Player("Player 3"),
                new Player("Player 4")
            };

            // Act
            _bettingLogic.ForceOtherPlayersToPass(players, 1);

            // Assert
            Assert.True(players[0].HasPassed);
            Assert.False(players[1].HasPassed);
            Assert.True(players[2].HasPassed);
            Assert.True(players[3].HasPassed);
        }

        [Fact]
        public void GetActivePlayers_ReturnsOnlyNonPassedPlayers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Player 1") { HasPassed = false },
                new Player("Player 2") { HasPassed = true },
                new Player("Player 3") { HasPassed = false },
                new Player("Player 4") { HasPassed = true }
            };

            // Act
            var activePlayers = _bettingLogic.GetActivePlayers(players);

            // Assert
            Assert.Equal(2, activePlayers.Count);
            Assert.Contains(players[0], activePlayers);
            Assert.Contains(players[2], activePlayers);
        }

        [Fact]
        public void CheckIfOnlyOnePlayerRemains_ReturnsTrueWhenOnePlayer()
        {
            // Arrange
            var activePlayers = new List<Player> { new Player("Player 1") };

            // Act
            var result = _bettingLogic.CheckIfOnlyOnePlayerRemains(activePlayers);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckIfOnlyOnePlayerRemains_ReturnsFalseWhenMultiplePlayers()
        {
            // Arrange
            var activePlayers = new List<Player>
            {
                new Player("Player 1"),
                new Player("Player 2")
            };

            // Act
            var result = _bettingLogic.CheckIfOnlyOnePlayerRemains(activePlayers);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void NoBetsPlaced_ReturnsTrueWhenNoBets()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Player 1"),
                new Player("Player 2"),
                new Player("Player 3"),
                new Player("Player 4")
            };

            // Act
            var result = _bettingLogic.NoBetsPlaced(players);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void NoBetsPlaced_ReturnsFalseWhenBetsExist()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Player 1") { HasBet = true, CurrentBid = 55 },
                new Player("Player 2"),
                new Player("Player 3"),
                new Player("Player 4")
            };

            // Act
            var result = _bettingLogic.NoBetsPlaced(players);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ForceMinimumBet_SetsPlayerToMinimumBet()
        {
            // Arrange
            var player = new Player("Test Player");

            // Act
            _bettingLogic.ForceMinimumBet(player);

            // Assert
            Assert.True(player.HasBet);
            Assert.Equal(50, player.CurrentBid);
        }

        [Fact]
        public void ForcePlayerToPass_MarksPlayerAsPassed()
        {
            // Arrange
            var player = new Player("Test Player");

            // Act
            _bettingLogic.ForcePlayerToPass(player);

            // Assert
            Assert.True(player.HasPassed);
            Assert.Equal(-1, player.CurrentBid);
        }

        [Fact]
        public void DetermineWinningBid_ReturnsHighestBid()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Player 1") { CurrentBid = 55 },
                new Player("Player 2") { CurrentBid = 75 },
                new Player("Player 3") { CurrentBid = 60 },
                new Player("Player 4") { CurrentBid = 0 }
            };

            // Act
            var (winningBid, winningBidIndex) = _bettingLogic.DetermineWinningBid(players);

            // Assert
            Assert.Equal(75, winningBid);
            Assert.Equal(1, winningBidIndex);
        }

        [Fact]
        public void DetermineWinningBid_ReturnsZeroWhenNoBids()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Player 1"),
                new Player("Player 2"),
                new Player("Player 3"),
                new Player("Player 4")
            };

            // Act
            var (winningBid, winningBidIndex) = _bettingLogic.DetermineWinningBid(players);

            // Assert
            Assert.Equal(0, winningBid);
            Assert.Equal(-1, winningBidIndex);
        }

        [Fact]
        public void DetermineWinningBid_IgnoresNegativeBids()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Player 1") { CurrentBid = -1 },
                new Player("Player 2") { CurrentBid = 65 },
                new Player("Player 3") { CurrentBid = -1 },
                new Player("Player 4") { CurrentBid = 0 }
            };

            // Act
            var (winningBid, winningBidIndex) = _bettingLogic.DetermineWinningBid(players);

            // Assert
            Assert.Equal(65, winningBid);
            Assert.Equal(1, winningBidIndex);
        }
    }
}