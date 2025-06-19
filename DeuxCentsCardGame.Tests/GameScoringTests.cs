using System.Reflection;
using Moq;

namespace DeuxCentsCardGame.Tests
{
    public class GameScoringTests
    {
        private readonly Mock<IUIConsoleGameView> _mockUI;
        
        public GameScoringTests()
        {
            _mockUI = new Mock<IUIConsoleGameView>();
            _mockUI.Setup(ui => ui.DisplayMessage(It.IsAny<string>())).Verifiable();
            _mockUI.Setup(ui => ui.DisplayFormattedMessage(
                    It.IsAny<string>(),
                    It.IsAny<object[]>()
                )).Verifiable();
        }
        
        private Game CreateGameInstance()
        {
            var game = new Game(_mockUI.Object);
            return game;
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
        
        private void SetBettingStateFields(Game game, int currentWinningBid, int currentWinningBidIndex, bool[] playerHasBet)
        {
            var bettingStateField = typeof(Game).GetField("_bettingState", BindingFlags.NonPublic | BindingFlags.Instance);
            var bettingState = bettingStateField.GetValue(game);
            
            SetPrivateProperty(bettingState, "CurrentWinningBid", currentWinningBid);
            SetPrivateProperty(bettingState, "CurrentWinningBidIndex", currentWinningBidIndex);
            SetPrivateProperty(bettingState, "PlayerHasBet", playerHasBet.ToList());
        }

        private void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var property = obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
            }
            else
            {
                throw new ArgumentException($"Property {propertyName} not found or not writable");
            }
        }

        private (int teamOneTotalPoints, int teamTwoTotalPoints) GetTeamTotalPoints(Game game)
        {
            var teamOneTotalPoints = (int)typeof(Game)
                .GetField("_teamOneTotalPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);
                
            var teamTwoTotalPoints = (int)typeof(Game)
                .GetField("_teamTwoTotalPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(game);
                
            return (teamOneTotalPoints, teamTwoTotalPoints);
        }

        private void ScoreBothTeams(Game game, bool teamOneBid)
        {
            var scoreBidWinningTeamMethod = typeof(Game).GetMethod("ScoreBidWinningTeam", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var scoreBidLosingTeamMethod = typeof(Game).GetMethod("ScoreBidLosingTeam", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (teamOneBid)
            {
                // Team One is bidding team, Team Two is losing team
                scoreBidWinningTeamMethod.Invoke(game, new object[] { true });
                scoreBidLosingTeamMethod.Invoke(game, new object[] { false });
            }
            else
            {
                // Team Two is bidding team, Team One is losing team
                scoreBidWinningTeamMethod.Invoke(game, new object[] { false });
                scoreBidLosingTeamMethod.Invoke(game, new object[] { true });
            }
        }

        [Fact]
        public void CompleteRoundScoring_WhenTeamOneBidsAndMakes_BothTeamsScoreCorrectly()
        {
            // Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 50);
            SetPrivateField(game, "_teamTwoTotalPoints", 60);
            SetPrivateField(game, "_teamOneRoundPoints", 60); // Team One made their bid
            SetPrivateField(game, "_teamTwoRoundPoints", 40); // Team Two gets their round points

            var playerHasBet = new bool[4] { true, false, false, false }; // Team One bet
            SetBettingStateFields(game, 50, 0, playerHasBet); // Team One won the bid

            int initialTeamOneTotal = 50;
            int initialTeamTwoTotal = 60;

            // When or Act - Score both teams
            ScoreBothTeams(game, teamOneBid: true);

            var (finalTeamOneTotal, finalTeamTwoTotal) = GetTeamTotalPoints(game);

            // Then or Assert
            Assert.Equal(initialTeamOneTotal + 60, finalTeamOneTotal); // Team One gets their round points
            Assert.Equal(initialTeamTwoTotal + 40, finalTeamTwoTotal); // Team Two gets their round points
        }

        [Fact]
        public void CompleteRoundScoring_WhenTeamOneBidsAndFails_BothTeamsScoreCorrectly()
        {
            // Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 50);
            SetPrivateField(game, "_teamTwoTotalPoints", 60);
            SetPrivateField(game, "_teamOneRoundPoints", 40); // Team One failed their bid (40 < 50)
            SetPrivateField(game, "_teamTwoRoundPoints", 60); // Team Two gets their round points

            var playerHasBet = new bool[4] { true, false, false, false }; // Team One bet
            SetBettingStateFields(game, 50, 0, playerHasBet); // Team One won the bid but failed to make it

            int initialTeamOneTotal = 50;
            int initialTeamTwoTotal = 60;

            // When or Act - Score both teams
            ScoreBothTeams(game, teamOneBid: true);

            var (finalTeamOneTotal, finalTeamTwoTotal) = GetTeamTotalPoints(game);

            // Then or Assert
            Assert.Equal(initialTeamOneTotal - 50, finalTeamOneTotal); // Team One loses their bid amount
            Assert.Equal(initialTeamTwoTotal + 60, finalTeamTwoTotal); // Team Two still gets their round points
        }

        [Fact]
        public void CompleteRoundScoring_WhenTeamTwoBidsAndMakes_BothTeamsScoreCorrectly()
        {
            // Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 50);
            SetPrivateField(game, "_teamTwoTotalPoints", 60);
            SetPrivateField(game, "_teamOneRoundPoints", 35); // Team One gets their round points
            SetPrivateField(game, "_teamTwoRoundPoints", 65); // Team Two made their bid

            var playerHasBet = new bool[4] { false, true, false, false }; // Team Two bet
            SetBettingStateFields(game, 50, 1, playerHasBet); // Team Two won the bid

            int initialTeamOneTotal = 50;
            int initialTeamTwoTotal = 60;

            // When or Act - Score both teams
            ScoreBothTeams(game, teamOneBid: false);

            var (finalTeamOneTotal, finalTeamTwoTotal) = GetTeamTotalPoints(game);

            // Then or Assert
            Assert.Equal(initialTeamOneTotal + 35, finalTeamOneTotal); // Team One gets their round points
            Assert.Equal(initialTeamTwoTotal + 65, finalTeamTwoTotal); // Team Two gets their round points
        }

        [Fact]
        public void CompleteRoundScoring_WhenBiddingTeamOver100AndDidNotBet_BiddingTeamScoresZero()
        {
            // Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 120); // Over 100
            SetPrivateField(game, "_teamTwoTotalPoints", 80);
            SetPrivateField(game, "_teamOneRoundPoints", 60); // Team One collected points but didn't bet
            SetPrivateField(game, "_teamTwoRoundPoints", 40);

            var playerHasBet = new bool[4] { false, false, false, false }; // Nobody bet (shouldn't happen in real game, but testing the logic)
            SetBettingStateFields(game, 50, 0, playerHasBet); // Team One "won" but didn't actually bet

            int initialTeamOneTotal = 120;
            int initialTeamTwoTotal = 80;

            // When or Act - Score both teams
            ScoreBothTeams(game, teamOneBid: true);

            var (finalTeamOneTotal, finalTeamTwoTotal) = GetTeamTotalPoints(game);

            // Then or Assert
            Assert.Equal(initialTeamOneTotal, finalTeamOneTotal); // Team One scores 0 (no change)
            Assert.Equal(initialTeamTwoTotal + 40, finalTeamTwoTotal); // Team Two still gets their points
        }

        [Fact]
        public void CompleteRoundScoring_WhenNonBiddingTeamOver100AndDidNotBet_NonBiddingTeamScoresZero()
        {
            // Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 80);
            SetPrivateField(game, "_teamTwoTotalPoints", 120); // Over 100
            SetPrivateField(game, "_teamOneRoundPoints", 60); // Team One made their bid
            SetPrivateField(game, "_teamTwoRoundPoints", 40); // Team Two collected points but didn't bet

            var playerHasBet = new bool[4] { true, false, false, false }; // Only Team One bet
            SetBettingStateFields(game, 50, 0, playerHasBet); // Team One won the bid

            int initialTeamOneTotal = 80;
            int initialTeamTwoTotal = 120;

            // When or Act - Score both teams
            ScoreBothTeams(game, teamOneBid: true);

            var (finalTeamOneTotal, finalTeamTwoTotal) = GetTeamTotalPoints(game);

            // Then or Assert
            Assert.Equal(initialTeamOneTotal + 60, finalTeamOneTotal); // Team One gets their points (made bid)
            Assert.Equal(initialTeamTwoTotal, finalTeamTwoTotal); // Team Two scores 0 (no change, over 100 and didn't bet)
        }

        [Fact]
        public void CompleteRoundScoring_WhenBothTeamsOver100AndNeitherBet_BothTeamsScoreZero()
        {
            // Given or Arrange
            var game = CreateGameInstance();

            SetPrivateField(game, "_teamOneTotalPoints", 120);
            SetPrivateField(game, "_teamTwoTotalPoints", 110);
            SetPrivateField(game, "_teamOneRoundPoints", 60);
            SetPrivateField(game, "_teamTwoRoundPoints", 40);

            var playerHasBet = new bool[4] { false, false, false, false }; // Neither team bet
            SetBettingStateFields(game, 50, 0, playerHasBet);

            int initialTeamOneTotal = 120;
            int initialTeamTwoTotal = 110;

            // When or Act - Score both teams
            ScoreBothTeams(game, teamOneBid: true);

            var (finalTeamOneTotal, finalTeamTwoTotal) = GetTeamTotalPoints(game);

            // Then or Assert
            Assert.Equal(initialTeamOneTotal, finalTeamOneTotal); // Team One scores 0
            Assert.Equal(initialTeamTwoTotal, finalTeamTwoTotal); // Team Two scores 0
        }
    }
}