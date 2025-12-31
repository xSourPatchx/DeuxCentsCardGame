using Moq;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Interfaces.Validators;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class PlayerManagerTests
    {
        private readonly Mock<ICardUtility> _mockCardUtility;
        private readonly Mock<IGameValidator> _mockGameValidator;

        public PlayerManagerTests()
        {
            _mockCardUtility = new Mock<ICardUtility>();
            _mockGameValidator = new Mock<IGameValidator>();

            // Setup card utility to return standard card game values
            _mockCardUtility.Setup(x => x.GetAllCardSuits())
                .Returns(new[] { CardSuit.Clubs, CardSuit.Diamonds, CardSuit.Hearts, CardSuit.Spades });
            _mockCardUtility.Setup(x => x.GetAllCardFaces())
                .Returns(new[] { CardFace.Five, CardFace.Six, CardFace.Seven, CardFace.Eight, CardFace.Nine,
                                CardFace.Ten, CardFace.Jack, CardFace.Queen, CardFace.King, CardFace.Ace });
            _mockCardUtility.Setup(x => x.GetCardFaceValues())
                .Returns(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _mockCardUtility.Setup(x => x.GetCardPointValues())
                .Returns(new[] { 0, 0, 0, 0, 0, 10, 2, 3, 4, 10 });
        }

        private GameEventManager CreateEventManager()
        {
            return new GameEventManager();
        }

        private Mock<IGameConfig> CreateMockGameConfig()
        {
            var mockConfig = new Mock<IGameConfig>();
            mockConfig.Setup(c => c.GetPlayerTypes())
                .Returns(new[] { PlayerType.Human, PlayerType.Human, PlayerType.Human, PlayerType.Human });
            return mockConfig;
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_InitializesFourPlayers()
        {
            // Arrange & Act
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Assert
            Assert.Equal(4, playerManager.Players.Count);
        }

        [Fact]
        public void Constructor_InitializesPlayersWithCorrectNames()
        {
            // Arrange & Act
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Assert
            Assert.Equal("Player 1", playerManager.Players[0].Name);
            Assert.Equal("Player 2", playerManager.Players[1].Name);
            Assert.Equal("Player 3", playerManager.Players[2].Name);
            Assert.Equal("Player 4", playerManager.Players[3].Name);
        }

        [Fact]
        public void Constructor_WithAIPlayers_InitializesWithCPUNames()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var mockConfig = new Mock<IGameConfig>();
            mockConfig.Setup(c => c.GetPlayerTypes())
                .Returns(new[] { PlayerType.Human, PlayerType.AI, PlayerType.Human, PlayerType.AI });

            // Act
            var playerManager = new PlayerManager(eventManager, mockConfig.Object);

            // Assert
            Assert.Equal("Player 1", playerManager.Players[0].Name);
            Assert.Equal("CPU 2", playerManager.Players[1].Name);
            Assert.Equal("Player 3", playerManager.Players[2].Name);
            Assert.Equal("CPU 4", playerManager.Players[3].Name);
        }

        [Fact]
        public void Constructor_InitializesPlayersWithEmptyHands()
        {
            // Arrange & Act
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Assert
            foreach (var player in playerManager.Players)
            {
                Assert.Empty(player.Hand);
            }
        }

        [Fact]
        public void Constructor_InitializesPlayersWithDefaultBettingState()
        {
            // Arrange & Act
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Assert
            foreach (var player in playerManager.Players)
            {
                Assert.False(player.HasBet);
                Assert.False(player.HasPassed);
                Assert.Equal(0, player.CurrentBid);
            }
        }

        #endregion

        #region Players Property Tests

        [Fact]
        public void Players_PropertyReturnsReadOnlyList()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Act
            var players = playerManager.Players;

            // Assert
            Assert.IsAssignableFrom<IReadOnlyList<Player>>(players);
        }

        [Fact]
        public void Players_CannotBeModifiedDirectly()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);
            var players = playerManager.Players;

            // Act & Assert
            // This should not compile if uncommented:
            // players.Add(new Player("Player 5"));
            // players.RemoveAt(0);
            Assert.Equal(4, players.Count);
        }

        #endregion

        #region GetPlayer Tests

        [Theory]
        [InlineData(0, "Player 1")]
        [InlineData(1, "Player 2")]
        [InlineData(2, "Player 3")]
        [InlineData(3, "Player 4")]
        public void GetPlayer_WithValidIndex_ReturnsCorrectPlayer(int index, string expectedName)
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Act
            var player = playerManager.GetPlayer(index);

            // Assert
            Assert.Equal(expectedName, player.Name);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetPlayer_WithInvalidIndex_ThrowsException(int index)
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => playerManager.GetPlayer(index));
        }

        [Fact]
        public void GetPlayer_ReturnsSameInstanceOnMultipleCalls()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Act
            var player1 = playerManager.GetPlayer(0);
            var player2 = playerManager.GetPlayer(0);

            // Assert
            Assert.Same(player1, player2);
        }

        #endregion

        #region ResetAllPlayerBettingStates Tests

        [Fact]
        public async Task ResetAllPlayerBettingStates_ResetsAllPlayersBettingProperties()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Set up players with betting states
            playerManager.GetPlayer(0).HasBet = true;
            playerManager.GetPlayer(0).CurrentBid = 100;
            playerManager.GetPlayer(1).HasPassed = true;
            playerManager.GetPlayer(2).HasBet = true;
            playerManager.GetPlayer(2).CurrentBid = 80;
            playerManager.GetPlayer(2).HasPassed = true;

            // Act
            await playerManager.ResetAllPlayerBettingStates();

            // Assert
            foreach (var player in playerManager.Players)
            {
                Assert.False(player.HasBet);
                Assert.False(player.HasPassed);
                Assert.Equal(0, player.CurrentBid);
            }
        }

        [Fact]
        public async Task ResetAllPlayerBettingStates_DoesNotAffectPlayerHands()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            
            playerManager.GetPlayer(0).AddCard(card);
            playerManager.GetPlayer(0).HasBet = true;
            playerManager.GetPlayer(0).CurrentBid = 100;

            // Act
            await playerManager.ResetAllPlayerBettingStates();

            // Assert
            Assert.Single(playerManager.GetPlayer(0).Hand);
            Assert.Contains(card, playerManager.GetPlayer(0).Hand);
        }

        [Fact]
        public async Task ResetAllPlayerBettingStates_CanBeCalledMultipleTimes()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            playerManager.GetPlayer(0).HasBet = true;
            playerManager.GetPlayer(0).CurrentBid = 100;

            // Act
            await playerManager.ResetAllPlayerBettingStates();
            await playerManager.ResetAllPlayerBettingStates();
            await playerManager.ResetAllPlayerBettingStates();

            // Assert
            Assert.False(playerManager.GetPlayer(0).HasBet);
            Assert.Equal(0, playerManager.GetPlayer(0).CurrentBid);
        }

        #endregion

        #region ClearAllPlayerHands Tests

        [Fact]
        public async Task ClearAllPlayerHands_RemovesAllCardsFromAllPlayers()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);
            var deck = new Deck(_mockCardUtility.Object, _mockGameValidator.Object);

            // Add cards to all players
            for (int i = 0; i < 4; i++)
            {
                playerManager.GetPlayer(i).AddCard(deck.Cards[i]);
                playerManager.GetPlayer(i).AddCard(deck.Cards[i + 4]);
            }

            // Act
            await playerManager.ClearAllPlayerHands();

            // Assert
            foreach (var player in playerManager.Players)
            {
                Assert.Empty(player.Hand);
            }
        }

        [Fact]
        public async Task ClearAllPlayerHands_DoesNotAffectBettingState()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);

            playerManager.GetPlayer(0).AddCard(card);
            playerManager.GetPlayer(0).HasBet = true;
            playerManager.GetPlayer(0).CurrentBid = 100;
            playerManager.GetPlayer(1).HasPassed = true;

            // Act
            await playerManager.ClearAllPlayerHands();

            // Assert
            Assert.True(playerManager.GetPlayer(0).HasBet);
            Assert.Equal(100, playerManager.GetPlayer(0).CurrentBid);
            Assert.True(playerManager.GetPlayer(1).HasPassed);
        }

        [Fact]
        public async Task ClearAllPlayerHands_CanBeCalledWhenHandsAlreadyEmpty()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () => 
                await playerManager.ClearAllPlayerHands());
            Assert.Null(exception);
            
            foreach (var player in playerManager.Players)
            {
                Assert.Empty(player.Hand);
            }
        }

        #endregion

        #region InitializePlayersWithTypes Tests

        [Fact]
        public void InitializePlayersWithTypes_CreatesCorrectPlayerTypes()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Act
            playerManager.InitializePlayersWithTypes(
                PlayerType.Human, 
                PlayerType.AI, 
                PlayerType.AI, 
                PlayerType.Human);

            // Assert
            Assert.Equal(PlayerType.Human, playerManager.GetPlayer(0).PlayerType);
            Assert.Equal(PlayerType.AI, playerManager.GetPlayer(1).PlayerType);
            Assert.Equal(PlayerType.AI, playerManager.GetPlayer(2).PlayerType);
            Assert.Equal(PlayerType.Human, playerManager.GetPlayer(3).PlayerType);
        }

        [Fact]
        public void InitializePlayersWithTypes_WithWrongCount_ThrowsException()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                playerManager.InitializePlayersWithTypes(PlayerType.Human, PlayerType.AI));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task PlayerManager_CompleteGameRoundScenario()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);
            var deck = new Deck(_mockCardUtility.Object, _mockGameValidator.Object);

            // Act - Simulate a complete round
            // 1. Deal cards
            for (int i = 0; i < 10; i++)
            {
                playerManager.GetPlayer(i % 4).AddCard(deck.Cards[i]);
            }

            // 2. Players place bets
            playerManager.GetPlayer(0).HasBet = true;
            playerManager.GetPlayer(0).CurrentBid = 100;
            playerManager.GetPlayer(1).HasPassed = true;
            playerManager.GetPlayer(2).HasPassed = true;
            playerManager.GetPlayer(3).HasPassed = true;

            // 3. Play cards
            var player0Cards = playerManager.GetPlayer(0).Hand.Count;

            // 4. Reset for next round
            await playerManager.ResetAllPlayerBettingStates();
            await playerManager.ClearAllPlayerHands();

            // Assert
            Assert.False(playerManager.GetPlayer(0).HasBet);
            Assert.Equal(0, playerManager.GetPlayer(0).CurrentBid);
            Assert.Empty(playerManager.GetPlayer(0).Hand);
        }

        [Fact]
        public void PlayerManager_MaintainsPlayerIdentity()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var gameConfig = CreateMockGameConfig();
            var playerManager = new PlayerManager(eventManager, gameConfig.Object);
            
            // Act - Get same player multiple times and modify
            var player1a = playerManager.GetPlayer(0);
            player1a.HasBet = true;
            
            var player1b = playerManager.GetPlayer(0);

            // Assert
            Assert.Same(player1a, player1b);
            Assert.True(player1b.HasBet);
        }

        #endregion
    }
}