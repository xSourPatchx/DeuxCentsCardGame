using Moq;
using Xunit;

namespace DeuxCentsCardGame.Tests
{
    public class GameTests
    {
        private readonly Mock<IUIConsoleGameView> _mockUI;
        private readonly Game _game;

        public GameTests()
        {
            _mockUI = new Mock<IUIConsoleGameView>();
            _game = new Game(_mockUI.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeGame()
        {
            // Assert
            Assert.NotNull(_game);
            Assert.Equal(4, _game.GetPlayers().Count);
            Assert.Equal(3, _game.DealerIndex); // Initial dealer should be Player 4 (index 3)
        }

        [Fact]
        public void NewRound_ShouldResetRoundState()
        {
            // Arrange
            _game.DealerIndex = 1; // Change from default

            // Act
            _game.NewRound();

            // Assert
            Assert.Equal(0, _game.GetTeamOneRoundPoints());
            Assert.Equal(0, _game.GetTeamTwoRoundPoints());
            Assert.Null(_game.GetTrumpSuit());
            Assert.NotNull(_game.GetBettingState());
        }

        [Fact]
        public void RotateDealer_ShouldIncrementDealerIndex()
        {
            // Arrange
            int initialDealerIndex = _game.DealerIndex;

            // Act
            _game.RotateDealer();

            // Assert
            Assert.Equal((initialDealerIndex + 1) % 4, _game.DealerIndex);
        }

        [Fact]
        public void DealCards_ShouldDistributeCardsEvenly()
        {
            // Act
            _game.DealCards();

            // Assert
            var players = _game.GetPlayers();
            Assert.Equal(13, players[0].Hand.Count);
            Assert.Equal(13, players[1].Hand.Count);
            Assert.Equal(13, players[2].Hand.Count);
            Assert.Equal(13, players[3].Hand.Count);
        }

        [Fact]
        public void SelectTrumpSuit_ShouldSetTrumpSuit()
        {
            // Arrange
            _mockUI.Setup(ui => ui.GetOptionInput(It.IsAny<string>(), It.IsAny<string[]>()))
                   .Returns("hearts");

            // Act
            _game.SelectTrumpSuit();

            // Assert
            Assert.Equal(CardSuit.Hearts, _game.GetTrumpSuit());
        }

        [Fact]
        public void IsPlayerOnTeamOne_ShouldReturnCorrectTeam()
        {
            // Players 1 and 3 (indices 0 and 2) are Team One
            Assert.True(_game.IsPlayerOnTeamOne(0));
            Assert.False(_game.IsPlayerOnTeamOne(1));
            Assert.True(_game.IsPlayerOnTeamOne(2));
            Assert.False(_game.IsPlayerOnTeamOne(3));
        }

        [Fact]
        public void AwardTrickPoints_ShouldAddPointsToCorrectTeam()
        {
            // Arrange
            int initialTeamOnePoints = _game.GetTeamOneRoundPoints();
            int initialTeamTwoPoints = _game.GetTeamTwoRoundPoints();
            int trickPoints = 20;

            // Act - Team One player (index 0)
            _game.AwardTrickPoints(0, trickPoints);

            // Assert
            Assert.Equal(initialTeamOnePoints + trickPoints, _game.GetTeamOneRoundPoints());
            Assert.Equal(initialTeamTwoPoints, _game.GetTeamTwoRoundPoints());

            // Act - Team Two player (index 1)
            _game.AwardTrickPoints(1, trickPoints);

            // Assert
            Assert.Equal(initialTeamOnePoints + trickPoints, _game.GetTeamOneRoundPoints());
            Assert.Equal(initialTeamTwoPoints + trickPoints, _game.GetTeamTwoRoundPoints());
        }

        [Fact]
        public void EndGameCheck_ShouldEndGameWhenScoreReached()
        {
            // Arrange
            _game.SetTeamOneTotalPoints(200);

            // Act
            _game.EndGameCheck();

            // Assert
            Assert.True(_game.IsGameEnded());
            _mockUI.Verify(ui => ui.DisplayMessage("Game over!"), Times.Once);
        }

        [Fact]
        public void ScoreRound_ShouldHandleBidWinningTeamSuccess()
        {
            // Arrange
            _game.SetBettingState(new BettingState(_game.GetPlayers(), _mockUI.Object, 0)
            {
                CurrentWinningBid = 80,
                CurrentWinningBidIndex = 0, // Team One player
                PlayerHasBet = new List<bool> { true, false, false, false }
            });
            _game.SetTeamOneRoundPoints(100);

            // Act
            _game.ScoreRound();

            // Assert
            Assert.Equal(100, _game.GetTeamOneTotalPoints()); // Should add the round points
            _mockUI.Verify(ui => ui.DisplayFormattedMessage("Team One made their bet of 80 and wins 100 points."), Times.Once);
        }

        [Fact]
        public void ScoreRound_ShouldHandleBidWinningTeamFailure()
        {
            // Arrange
            _game.SetBettingState(new BettingState(_game.GetPlayers(), _mockUI.Object, 0)
            {
                CurrentWinningBid = 80,
                CurrentWinningBidIndex = 0, // Team One player
                PlayerHasBet = new List<bool> { true, false, false, false }
            });
            _game.SetTeamOneRoundPoints(60);
            _game.SetTeamOneTotalPoints(50);

            // Act
            _game.ScoreRound();

            // Assert
            Assert.Equal(50 - 80, _game.GetTeamOneTotalPoints()); // Should subtract the bid amount
            _mockUI.Verify(ui => ui.DisplayFormattedMessage("Team One did not make their bet of 80 and loses 80 points."), Times.Once);
        }

        // Helper methods would need to be added to the Game class for testing purposes:
        /*
        public List<Player> GetPlayers() => _players;
        public int GetTeamOneRoundPoints() => _teamOneRoundPoints;
        public int GetTeamTwoRoundPoints() => _teamTwoRoundPoints;
        public int GetTeamOneTotalPoints() => _teamOneTotalPoints;
        public int GetTeamTwoTotalPoints() => _teamTwoTotalPoints;
        public CardSuit? GetTrumpSuit() => _trumpSuit;
        public BettingState GetBettingState() => _bettingState;
        public bool IsGameEnded() => _isGameEnded;
        public void SetTeamOneTotalPoints(int points) => _teamOneTotalPoints = points;
        public void SetTeamTwoTotalPoints(int points) => _teamTwoTotalPoints = points;
        public void SetBettingState(BettingState state) => _bettingState = state;
        */
    }
}