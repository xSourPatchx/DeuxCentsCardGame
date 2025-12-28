using Moq;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Gameplay;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class ScoringManagerTests
    {
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly Mock<ITeamManager> _mockTeamManager;
        private readonly Mock<ScoringLogic> _mockScoringLogic;
        private readonly List<Player> _players;
        private readonly ScoringManager _scoringManager;

        public ScoringManagerTests()
        {
            _mockEventManager = new Mock<IGameEventManager>();
            _mockTeamManager = new Mock<ITeamManager>();
            _mockScoringLogic = new Mock<ScoringLogic>(200, 100);
            
            _players = new List<Player>
            {
                new Player("Player1"),
                new Player("Player2"),
                new Player("Player3"),
                new Player("Player4")
            };

            _scoringManager = new ScoringManager(
                _mockEventManager.Object, 
                _players, 
                _mockTeamManager.Object, 
                _mockScoringLogic.Object);
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
            _mockEventManager.Verify(x => x.RaiseTrickPointsAwarded(_players[0], 20, "Team One"), Times.Once);
        }

        [Fact]
        public void ScoreRound_CallsScoringLogicAndRaisesEvents()
        {
            // Arrange
            int winningBid = 50;
            int winningBidIndex = 0;
            
            _mockTeamManager.Setup(x => x.IsPlayerOnTeamOne(winningBidIndex)).Returns(true);
            _mockTeamManager.Setup(x => x.GetTeamPlayerIndices(Team.TeamOne))
                .Returns((0, 2));
            _mockTeamManager.Setup(x => x.GetTeamPlayerIndices(Team.TeamTwo))
                .Returns((1, 3));
            _mockTeamManager.Setup(x => x.GetTeamName(Team.TeamOne)).Returns("Team One");
            _mockTeamManager.Setup(x => x.GetTeamName(Team.TeamTwo)).Returns("Team Two");

            _mockScoringLogic.Setup(x => x.DetermineIfTeamCannotScore(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(false);
            _mockScoringLogic.Setup(x => x.CalculateAwardedPoints(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>()))
                .Returns(50);
            _mockScoringLogic.Setup(x => x.DetermineBidSuccess(
                It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(true);

            // Act
            _scoringManager.ScoreRound(winningBidIndex, winningBid);

            // Assert
            _mockEventManager.Verify(x => x.RaiseScoreUpdated(
                It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<int>(),
                true, winningBid), Times.Once);
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

            _mockScoringLogic.Setup(x => x.DetermineIfTeamCannotScore(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(false);
            _mockScoringLogic.Setup(x => x.CalculateAwardedPoints(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>()))
                .Returns(100);
            _mockScoringLogic.Setup(x => x.DetermineBidSuccess(
                It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(true);
            _mockScoringLogic.Setup(x => x.IsGameOver(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((t1, t2) => t1 >= 200 || t2 >= 200);

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
            Assert.True(_scoringManager.TeamOneTotalPoints >= 200, 
                $"TeamOneTotalPoints was {_scoringManager.TeamOneTotalPoints}, expected >= 200");
            Assert.True(isGameOver);
        }

        [Fact]
        public void RaiseGameOverEvent_CallsEventManager()
        {
            // Act
            _scoringManager.RaiseGameOverEvent();

            // Assert
            _mockEventManager.Verify(x => x.RaiseGameOver(
                It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }
    }
}