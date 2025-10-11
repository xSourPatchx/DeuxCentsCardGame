using Moq;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class TeamManagerTests
    {
        private IGameConfig CreateMockGameConfig()
        {
            var mockConfig = new Mock<IGameConfig>();
            mockConfig.Setup(c => c.TeamOnePlayer1).Returns(0);
            mockConfig.Setup(c => c.TeamOnePlayer2).Returns(2);
            mockConfig.Setup(c => c.TeamTwoPlayer1).Returns(1);
            mockConfig.Setup(c => c.TeamTwoPlayer2).Returns(3);
            return mockConfig.Object;
        }

        private List<Player> CreatePlayers()
        {
            return new List<Player>
            {
                new Player("Player 1"),
                new Player("Player 2"),
                new Player("Player 3"),
                new Player("Player 4")
            };
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidGameConfig_CreatesTeamManager()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();

            // Act
            var teamManager = new TeamManager(gameConfig);

            // Assert
            Assert.NotNull(teamManager);
        }

        [Fact]
        public void Constructor_WithNullGameConfig_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new TeamManager(null));
        }

        #endregion

        #region GetPlayerTeam Tests

        [Theory]
        [InlineData(0, Team.TeamOne)]
        [InlineData(2, Team.TeamOne)]
        public void GetPlayerTeam_ForTeamOnePlayers_ReturnsTeamOne(int playerIndex, Team expectedTeam)
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var result = teamManager.GetPlayerTeam(playerIndex);

            // Assert
            Assert.Equal(expectedTeam, result);
        }

        [Theory]
        [InlineData(1, Team.TeamTwo)]
        [InlineData(3, Team.TeamTwo)]
        public void GetPlayerTeam_ForTeamTwoPlayers_ReturnsTeamTwo(int playerIndex, Team expectedTeam)
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var result = teamManager.GetPlayerTeam(playerIndex);

            // Assert
            Assert.Equal(expectedTeam, result);
        }

        #endregion

        #region IsPlayerOnTeamOne Tests

        [Theory]
        [InlineData(0, true)]
        [InlineData(2, true)]
        [InlineData(4, true)]
        [InlineData(100, true)]
        public void IsPlayerOnTeamOne_ForEvenIndices_ReturnsTrue(int playerIndex, bool expected)
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var result = teamManager.IsPlayerOnTeamOne(playerIndex);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(3, false)]
        [InlineData(5, false)]
        [InlineData(99, false)]
        public void IsPlayerOnTeamOne_ForOddIndices_ReturnsFalse(int playerIndex, bool expected)
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var result = teamManager.IsPlayerOnTeamOne(playerIndex);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region IsPlayerOnTeamTwo Tests

        [Theory]
        [InlineData(1, true)]
        [InlineData(3, true)]
        [InlineData(5, true)]
        [InlineData(99, true)]
        public void IsPlayerOnTeamTwo_ForOddIndices_ReturnsTrue(int playerIndex, bool expected)
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var result = teamManager.IsPlayerOnTeamTwo(playerIndex);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(2, false)]
        [InlineData(4, false)]
        [InlineData(100, false)]
        public void IsPlayerOnTeamTwo_ForEvenIndices_ReturnsFalse(int playerIndex, bool expected)
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var result = teamManager.IsPlayerOnTeamTwo(playerIndex);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region GetTeamPlayerIndices Tests

        [Fact]
        public void GetTeamPlayerIndices_ForTeamOne_ReturnsCorrectIndices()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var (player1, player2) = teamManager.GetTeamPlayerIndices(Team.TeamOne);

            // Assert
            Assert.Equal(0, player1);
            Assert.Equal(2, player2);
        }

        [Fact]
        public void GetTeamPlayerIndices_ForTeamTwo_ReturnsCorrectIndices()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var (player1, player2) = teamManager.GetTeamPlayerIndices(Team.TeamTwo);

            // Assert
            Assert.Equal(1, player1);
            Assert.Equal(3, player2);
        }

        [Fact]
        public void GetTeamPlayerIndices_WithInvalidTeam_ThrowsArgumentException()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                teamManager.GetTeamPlayerIndices((Team)999));
        }

        #endregion

        #region GetTeamPlayers Tests

        [Fact]
        public void GetTeamPlayers_ForTeamOne_ReturnsTwoPlayers()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);
            var allPlayers = CreatePlayers();

            // Act
            var teamPlayers = teamManager.GetTeamPlayers(Team.TeamOne, allPlayers);

            // Assert
            Assert.Equal(2, teamPlayers.Count);
            Assert.Equal("Player 1", teamPlayers[0].Name);
            Assert.Equal("Player 3", teamPlayers[1].Name);
        }

        [Fact]
        public void GetTeamPlayers_ForTeamTwo_ReturnsTwoPlayers()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);
            var allPlayers = CreatePlayers();

            // Act
            var teamPlayers = teamManager.GetTeamPlayers(Team.TeamTwo, allPlayers);

            // Assert
            Assert.Equal(2, teamPlayers.Count);
            Assert.Equal("Player 2", teamPlayers[0].Name);
            Assert.Equal("Player 4", teamPlayers[1].Name);
        }

        [Fact]
        public void GetTeamPlayers_ReturnsNewListInstance()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);
            var allPlayers = CreatePlayers();

            // Act
            var teamPlayers1 = teamManager.GetTeamPlayers(Team.TeamOne, allPlayers);
            var teamPlayers2 = teamManager.GetTeamPlayers(Team.TeamOne, allPlayers);

            // Assert
            Assert.NotSame(teamPlayers1, teamPlayers2);
            Assert.Equal(teamPlayers1.Count, teamPlayers2.Count);
        }

        #endregion

        #region GetTeamName Tests

        [Fact]
        public void GetTeamName_WithTeamOne_ReturnsTeamOne()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var teamName = teamManager.GetTeamName(Team.TeamOne);

            // Assert
            Assert.Equal("Team One", teamName);
        }

        [Fact]
        public void GetTeamName_WithTeamTwo_ReturnsTeamTwo()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var teamName = teamManager.GetTeamName(Team.TeamTwo);

            // Assert
            Assert.Equal("Team Two", teamName);
        }

        [Fact]
        public void GetTeamName_WithInvalidTeam_ThrowsArgumentException()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                teamManager.GetTeamName((Team)999));
        }

        [Theory]
        [InlineData(0, "Team One")]
        [InlineData(2, "Team One")]
        public void GetTeamName_WithTeamOnePlayerIndex_ReturnsTeamOne(int playerIndex, string expected)
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var teamName = teamManager.GetTeamName(playerIndex);

            // Assert
            Assert.Equal(expected, teamName);
        }

        [Theory]
        [InlineData(1, "Team Two")]
        [InlineData(3, "Team Two")]
        public void GetTeamName_WithTeamTwoPlayerIndex_ReturnsTeamTwo(int playerIndex, string expected)
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act
            var teamName = teamManager.GetTeamName(playerIndex);

            // Assert
            Assert.Equal(expected, teamName);
        }

        #endregion

        #region HasTeamPlacedBet Tests

        [Fact]
        public void HasTeamPlacedBet_WhenNoTeamMemberHasBet_ReturnsFalse()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);
            var allPlayers = CreatePlayers();

            // Act
            var result = teamManager.HasTeamPlacedBet(Team.TeamOne, allPlayers);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasTeamPlacedBet_WhenOneTeamMemberHasBet_ReturnsTrue()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);
            var allPlayers = CreatePlayers();
            allPlayers[0].HasBet = true;

            // Act
            var result = teamManager.HasTeamPlacedBet(Team.TeamOne, allPlayers);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasTeamPlacedBet_WhenBothTeamMembersHaveBet_ReturnsTrue()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);
            var allPlayers = CreatePlayers();
            allPlayers[0].HasBet = true;
            allPlayers[2].HasBet = true;

            // Act
            var result = teamManager.HasTeamPlacedBet(Team.TeamOne, allPlayers);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasTeamPlacedBet_OnlyChecksSpecifiedTeam()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);
            var allPlayers = CreatePlayers();
            allPlayers[1].HasBet = true; // Team Two player
            allPlayers[3].HasBet = true; // Team Two player

            // Act
            var teamOneResult = teamManager.HasTeamPlacedBet(Team.TeamOne, allPlayers);
            var teamTwoResult = teamManager.HasTeamPlacedBet(Team.TeamTwo, allPlayers);

            // Assert
            Assert.False(teamOneResult);
            Assert.True(teamTwoResult);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void TeamManager_CompleteTeamScenario()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);
            var allPlayers = CreatePlayers();

            // Act & Assert - Verify team structure
            Assert.Equal(Team.TeamOne, teamManager.GetPlayerTeam(0));
            Assert.Equal(Team.TeamOne, teamManager.GetPlayerTeam(2));
            Assert.Equal(Team.TeamTwo, teamManager.GetPlayerTeam(1));
            Assert.Equal(Team.TeamTwo, teamManager.GetPlayerTeam(3));

            // Verify team player retrieval
            var teamOnePlayers = teamManager.GetTeamPlayers(Team.TeamOne, allPlayers);
            Assert.Equal(2, teamOnePlayers.Count);
            Assert.Contains(allPlayers[0], teamOnePlayers);
            Assert.Contains(allPlayers[2], teamOnePlayers);

            // Verify betting logic
            allPlayers[0].HasBet = true;
            Assert.True(teamManager.HasTeamPlacedBet(Team.TeamOne, allPlayers));
            Assert.False(teamManager.HasTeamPlacedBet(Team.TeamTwo, allPlayers));
        }

        [Fact]
        public void TeamManager_TeamAssignmentsAreConsistent()
        {
            // Arrange
            var gameConfig = CreateMockGameConfig();
            var teamManager = new TeamManager(gameConfig);

            // Act & Assert - Multiple calls should return same results
            for (int i = 0; i < 10; i++)
            {
                Assert.True(teamManager.IsPlayerOnTeamOne(0));
                Assert.True(teamManager.IsPlayerOnTeamTwo(1));
                Assert.Equal(Team.TeamOne, teamManager.GetPlayerTeam(0));
                Assert.Equal(Team.TeamTwo, teamManager.GetPlayerTeam(1));
            }
        }

        #endregion
    }
}