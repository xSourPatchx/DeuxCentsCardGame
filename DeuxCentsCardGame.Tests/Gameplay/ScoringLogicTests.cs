using DeuxCentsCardGame.Gameplay;
using Moq;

namespace DeuxCentsCardGame.Tests.Gameplay
{
    public class ScoringLogicTests
    {
        private readonly ScoringLogic _scoringLogic;
        private readonly Mock<Interfaces.GameConfig.IGameConfig> _mockGameConfig;

        public ScoringLogicTests()
        {
            _mockGameConfig = new Mock<Interfaces.GameConfig.IGameConfig>();
            _mockGameConfig.Setup(c => c.CannotScoreThreshold).Returns(100);
            _mockGameConfig.Setup(c => c.WinningScore).Returns(200);
            
            _scoringLogic = new ScoringLogic(_mockGameConfig.Object);
        }

        #region DetermineIfTeamCannotScore Tests

        [Fact]
        public void DetermineIfTeamCannotScore_ReturnsTrueWhenOver100AndNoBets()
        {
            // Act
            var result = _scoringLogic.DetermineIfTeamCannotScore(105, false, false);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DetermineIfTeamCannotScore_ReturnsFalseWhenUnder100()
        {
            // Act
            var result = _scoringLogic.DetermineIfTeamCannotScore(95, false, false);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DetermineIfTeamCannotScore_ReturnsFalseWhenPlayer1HasBet()
        {
            // Act
            var result = _scoringLogic.DetermineIfTeamCannotScore(105, true, false);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DetermineIfTeamCannotScore_ReturnsFalseWhenPlayer2HasBet()
        {
            // Act
            var result = _scoringLogic.DetermineIfTeamCannotScore(105, false, true);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DetermineIfTeamCannotScore_ReturnsFalseWhenBothPlayersHaveBet()
        {
            // Act
            var result = _scoringLogic.DetermineIfTeamCannotScore(105, true, true);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DetermineIfTeamCannotScore_ReturnsFalseWhenExactly100()
        {
            // Act
            var result = _scoringLogic.DetermineIfTeamCannotScore(100, false, false);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region CalculateAwardedPoints Tests

        [Fact]
        public void CalculateAwardedPoints_ReturnsZeroWhenTeamCannotScore()
        {
            // Act
            var result = _scoringLogic.CalculateAwardedPoints(75, true, false, 60);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void CalculateAwardedPoints_ReturnsBidWinnerPointsWhenMadeBid()
        {
            // Act
            var result = _scoringLogic.CalculateAwardedPoints(75, false, true, 60);

            // Assert
            Assert.Equal(75, result);
        }

        [Fact]
        public void CalculateAwardedPoints_ReturnsNegativeBidWhenFailedBid()
        {
            // Act
            var result = _scoringLogic.CalculateAwardedPoints(55, false, true, 60);

            // Assert
            Assert.Equal(-60, result);
        }

        [Fact]
        public void CalculateAwardedPoints_ReturnsRoundPointsForNonBidWinner()
        {
            // Act
            var result = _scoringLogic.CalculateAwardedPoints(45, false, false, 60);

            // Assert
            Assert.Equal(45, result);
        }

        [Fact]
        public void CalculateAwardedPoints_BidWinnerExactlyMakesBid()
        {
            // Act
            var result = _scoringLogic.CalculateAwardedPoints(60, false, true, 60);

            // Assert
            Assert.Equal(60, result);
        }

        #endregion

        #region DetermineBidSuccess Tests

        [Fact]
        public void DetermineBidSuccess_ReturnsTrueWhenBidWinnerMakesBid()
        {
            // Act
            var result = _scoringLogic.DetermineBidSuccess(true, 75, 60);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DetermineBidSuccess_ReturnsFalseWhenBidWinnerFailsBid()
        {
            // Act
            var result = _scoringLogic.DetermineBidSuccess(true, 55, 60);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DetermineBidSuccess_ReturnsFalseWhenNotBidWinner()
        {
            // Act
            var result = _scoringLogic.DetermineBidSuccess(false, 75, 60);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DetermineBidSuccess_ReturnsTrueWhenExactlyMakesBid()
        {
            // Act
            var result = _scoringLogic.DetermineBidSuccess(true, 60, 60);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region IsGameOver Tests

        [Fact]
        public void IsGameOver_ReturnsTrueWhenTeamOneReaches200()
        {
            // Act
            var result = _scoringLogic.IsGameOver(200, 150);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsGameOver_ReturnsTrueWhenTeamTwoReaches200()
        {
            // Act
            var result = _scoringLogic.IsGameOver(150, 200);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsGameOver_ReturnsTrueWhenBothTeamsReach200()
        {
            // Act
            var result = _scoringLogic.IsGameOver(200, 205);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsGameOver_ReturnsFalseWhenBothTeamsUnder200()
        {
            // Act
            var result = _scoringLogic.IsGameOver(190, 185);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsGameOver_ReturnsTrueWhenTeamOneExceeds200()
        {
            // Act
            var result = _scoringLogic.IsGameOver(215, 150);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsGameOver_ReturnsFalseWhenJustBelow200()
        {
            // Act
            var result = _scoringLogic.IsGameOver(199, 199);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}