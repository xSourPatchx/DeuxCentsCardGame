using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class PlayerManagerTests
    {
        private GameEventManager CreateEventManager()
        {
            return new GameEventManager();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_InitializesFourPlayers()
        {
            // Arrange & Act
            var eventManager = CreateEventManager();
            var playerManager = new PlayerManager(eventManager);

            // Assert
            Assert.Equal(4, playerManager.Players.Count);
        }

        [Fact]
        public void Constructor_InitializesPlayersWithCorrectNames()
        {
            // Arrange & Act
            var eventManager = CreateEventManager();
            var playerManager = new PlayerManager(eventManager);

            // Assert
            Assert.Equal("Player 1", playerManager.Players[0].Name);
            Assert.Equal("Player 2", playerManager.Players[1].Name);
            Assert.Equal("Player 3", playerManager.Players[2].Name);
            Assert.Equal("Player 4", playerManager.Players[3].Name);
        }

        [Fact]
        public void Constructor_InitializesPlayersWithEmptyHands()
        {
            // Arrange & Act
            var eventManager = CreateEventManager();
            var playerManager = new PlayerManager(eventManager);

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
            var playerManager = new PlayerManager(eventManager);

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
            var playerManager = new PlayerManager(eventManager);

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
            var playerManager = new PlayerManager(eventManager);
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
            var playerManager = new PlayerManager(eventManager);

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
            var playerManager = new PlayerManager(eventManager);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => playerManager.GetPlayer(index));
        }

        [Fact]
        public void GetPlayer_ReturnsSameInstanceOnMultipleCalls()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var playerManager = new PlayerManager(eventManager);

            // Act
            var player1 = playerManager.GetPlayer(0);
            var player2 = playerManager.GetPlayer(0);

            // Assert
            Assert.Same(player1, player2);
        }

        #endregion

        #region ResetAllPlayerBettingStates Tests

        [Fact]
        public void ResetAllPlayerBettingStates_ResetsAllPlayersBettingProperties()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var playerManager = new PlayerManager(eventManager);

            // Set up players with betting states
            playerManager.GetPlayer(0).HasBet = true;
            playerManager.GetPlayer(0).CurrentBid = 100;
            playerManager.GetPlayer(1).HasPassed = true;
            playerManager.GetPlayer(2).HasBet = true;
            playerManager.GetPlayer(2).CurrentBid = 80;
            playerManager.GetPlayer(2).HasPassed = true;

            // Act
            playerManager.ResetAllPlayerBettingStates();

            // Assert
            foreach (var player in playerManager.Players)
            {
                Assert.False(player.HasBet);
                Assert.False(player.HasPassed);
                Assert.Equal(0, player.CurrentBid);
            }
        }

        [Fact]
        public void ResetAllPlayerBettingStates_DoesNotAffectPlayerHands()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var playerManager = new PlayerManager(eventManager);
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            
            playerManager.GetPlayer(0).AddCard(card);
            playerManager.GetPlayer(0).HasBet = true;
            playerManager.GetPlayer(0).CurrentBid = 100;

            // Act
            playerManager.ResetAllPlayerBettingStates();

            // Assert
            Assert.Single(playerManager.GetPlayer(0).Hand);
            Assert.Contains(card, playerManager.GetPlayer(0).Hand);
        }

        [Fact]
        public void ResetAllPlayerBettingStates_CanBeCalledMultipleTimes()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var playerManager = new PlayerManager(eventManager);

            playerManager.GetPlayer(0).HasBet = true;
            playerManager.GetPlayer(0).CurrentBid = 100;

            // Act
            playerManager.ResetAllPlayerBettingStates();
            playerManager.ResetAllPlayerBettingStates();
            playerManager.ResetAllPlayerBettingStates();

            // Assert
            Assert.False(playerManager.GetPlayer(0).HasBet);
            Assert.Equal(0, playerManager.GetPlayer(0).CurrentBid);
        }

        #endregion

        #region ClearAllPlayerHands Tests

        [Fact]
        public void ClearAllPlayerHands_RemovesAllCardsFromAllPlayers()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var playerManager = new PlayerManager(eventManager);
            var deck = new Deck();

            // Add cards to all players
            for (int i = 0; i < 4; i++)
            {
                playerManager.GetPlayer(i).AddCard(deck.Cards[i]);
                playerManager.GetPlayer(i).AddCard(deck.Cards[i + 4]);
            }

            // Act
            playerManager.ClearAllPlayerHands();

            // Assert
            foreach (var player in playerManager.Players)
            {
                Assert.Empty(player.Hand);
            }
        }

        [Fact]
        public void ClearAllPlayerHands_DoesNotAffectBettingState()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var playerManager = new PlayerManager(eventManager);
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);

            playerManager.GetPlayer(0).AddCard(card);
            playerManager.GetPlayer(0).HasBet = true;
            playerManager.GetPlayer(0).CurrentBid = 100;
            playerManager.GetPlayer(1).HasPassed = true;

            // Act
            playerManager.ClearAllPlayerHands();

            // Assert
            Assert.True(playerManager.GetPlayer(0).HasBet);
            Assert.Equal(100, playerManager.GetPlayer(0).CurrentBid);
            Assert.True(playerManager.GetPlayer(1).HasPassed);
        }

        [Fact]
        public void ClearAllPlayerHands_CanBeCalledWhenHandsAlreadyEmpty()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var playerManager = new PlayerManager(eventManager);

            // Act & Assert
            var exception = Record.Exception(() => playerManager.ClearAllPlayerHands());
            Assert.Null(exception);
            
            foreach (var player in playerManager.Players)
            {
                Assert.Empty(player.Hand);
            }
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void PlayerManager_CompleteGameRoundScenario()
        {
            // Arrange
            var eventManager = CreateEventManager();
            var playerManager = new PlayerManager(eventManager);
            var deck = new Deck();

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
            playerManager.ResetAllPlayerBettingStates();
            playerManager.ClearAllPlayerHands();

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
            var playerManager = new PlayerManager(eventManager);

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