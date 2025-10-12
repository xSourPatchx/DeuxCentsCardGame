using Moq;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class ScoringManagerTests
    {
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly Mock<ITeamManager> _mockTeamManager;
        private readonly Mock<IGameConfig> _mockGameConfig;
        private readonly List<Player> _players;
        private readonly ScoringManager _scoringManager;

        public ScoringManagerTests()
        {
            _mockEventManager = new Mock<IGameEventManager>();
            _mockTeamManager = new Mock<ITeamManager>();
            _mockGameConfig = new Mock<IGameConfig>();
            
            _players = new List<Player>
            {
                new Player("Player1"),
                new Player("Player2"),
                new Player("Player3"),
                new Player("Player4")
            };

            _mockGameConfig.Setup(x => x.WinningScore).Returns(200);
            _mockGameConfig.Setup(x => x.CannotScoreThreshold).Returns(100);

            _scoringManager = new ScoringManager(_mockEventManager.Object, _players, 
                                                _mockTeamManager.Object, _mockGameConfig.Object);
        }

        [Fact]
        public void ResetRoundPoints_ResetsPointsToZero()
        {
            // Act
            _scoringManager.ResetRoundPoints();

            // Assert
            Assert.Equal(0, _scoringManager.TeamOneRoundPoints);
            Assert.Equal(0, _scoringManager.TeamTwoRoundPoints);
        }

        [Fact]
        public void AwardTrickPoints_AddsPointsToCorrectTeam()
        {
            // Arrange
            _mockTeamManager.Setup(x => x.IsPlayerOnTeamOne(0)).Returns(true);
            _mockTeamManager.Setup(x => x.GetTeamName(0)).Returns("Team One");

            // Act
            _scoringManager.AwardTrickPoints(0, 20);

            // Assert
            Assert.Equal(20, _scoringManager.TeamOneRoundPoints);
            Assert.Equal(0, _scoringManager.TeamTwoRoundPoints);
        }

        [Fact]
        public void IsGameOver_WhenTeamReaches200_ReturnsTrue()
        {
            // Arrange
            _mockTeamManager.Setup(x => x.IsPlayerOnTeamOne(0)).Returns(true);
            _mockTeamManager.Setup(x => x.GetTeamName(0)).Returns("Team One");
            _mockTeamManager.Setup(x => x.GetTeamPlayerIndices(Team.TeamOne))
                .Returns((0, 2));
            _mockTeamManager.Setup(x => x.GetTeamPlayerIndices(Team.TeamTwo))
                .Returns((1, 3));
            _mockTeamManager.Setup(x => x.GetTeamName(Team.TeamOne)).Returns("Team One");
            _mockTeamManager.Setup(x => x.GetTeamName(Team.TeamTwo)).Returns("Team Two");

            // Award points to get Team One to 200
            while (_scoringManager.TeamOneTotalPoints < 200)
            {
                _scoringManager.ResetRoundPoints();
                _scoringManager.AwardTrickPoints(0, 100);
                _scoringManager.ScoreRound(0, 50);
            }

            // Act
            bool isGameOver = _scoringManager.IsGameOver();

            // Assert
            Assert.True(_scoringManager.TeamOneTotalPoints >= 200, $"TeamOneTotalPoints was {_scoringManager.TeamOneTotalPoints}, expected >= 200");
            Assert.True(isGameOver);
        }
    }
}