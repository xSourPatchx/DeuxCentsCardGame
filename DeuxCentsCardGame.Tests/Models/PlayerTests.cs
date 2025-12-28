using DeuxCentsCardGame.Models;
using Xunit;

namespace DeuxCentsCardGame.Tests.Models
{
    public class PlayerTests
    {
        [Fact]
        public void Player_Constructor_SetsNameAndDefaultsToHuman()
        {
            // Act
            var player = new Player("Test Player");

            // Assert
            Assert.Equal("Test Player", player.Name);
            Assert.Equal(PlayerType.Human, player.PlayerType);
            Assert.Empty(player.Hand);
            Assert.False(player.HasBet);
            Assert.False(player.HasPassed);
            Assert.Equal(0, player.CurrentBid);
        }

        [Fact]
        public void Player_Constructor_SetsPlayerTypeCorrectly()
        {
            // Act
            var aiPlayer = new Player("AI Player", PlayerType.AI);

            // Assert
            Assert.Equal("AI Player", aiPlayer.Name);
            Assert.Equal(PlayerType.AI, aiPlayer.PlayerType);
        }

        [Fact]
        public void AddCard_AddsCardToHand()
        {
            // Arrange
            var player = new Player("Test Player");
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);

            // Act
            player.AddCard(card);

            // Assert
            Assert.Single(player.Hand);
            Assert.Contains(card, player.Hand);
        }

        [Fact]
        public void AddCard_AddsMultipleCards()
        {
            // Arrange
            var player = new Player("Test Player");
            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Spades, CardFace.King, 9, 0);

            // Act
            player.AddCard(card1);
            player.AddCard(card2);

            // Assert
            Assert.Equal(2, player.Hand.Count);
            Assert.Contains(card1, player.Hand);
            Assert.Contains(card2, player.Hand);
        }

        [Fact]
        public void RemoveCard_RemovesCardFromHand()
        {
            // Arrange
            var player = new Player("Test Player");
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            player.AddCard(card);

            // Act
            player.RemoveCard(card);

            // Assert
            Assert.Empty(player.Hand);
        }

        [Fact]
        public void RemoveCard_OnlyRemovesSpecifiedCard()
        {
            // Arrange
            var player = new Player("Test Player");
            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Spades, CardFace.King, 9, 0);
            player.AddCard(card1);
            player.AddCard(card2);

            // Act
            player.RemoveCard(card1);

            // Assert
            Assert.Single(player.Hand);
            Assert.Contains(card2, player.Hand);
            Assert.DoesNotContain(card1, player.Hand);
        }

        [Fact]
        public void ResetBettingState_ResetsAllBettingProperties()
        {
            // Arrange
            var player = new Player("Test Player");
            player.HasBet = true;
            player.HasPassed = true;
            player.CurrentBid = 75;

            // Act
            player.ResetBettingState();

            // Assert
            Assert.False(player.HasBet);
            Assert.False(player.HasPassed);
            Assert.Equal(0, player.CurrentBid);
        }

        [Fact]
        public void IsHuman_ReturnsTrueForHumanPlayer()
        {
            // Arrange
            var player = new Player("Human Player", PlayerType.Human);

            // Act & Assert
            Assert.True(player.IsHuman());
            Assert.False(player.IsAI());
        }

        [Fact]
        public void IsAI_ReturnsTrueForAIPlayer()
        {
            // Arrange
            var player = new Player("AI Player", PlayerType.AI);

            // Act & Assert
            Assert.True(player.IsAI());
            Assert.False(player.IsHuman());
        }

        [Fact]
        public void Player_BettingState_CanBeModified()
        {
            // Arrange
            var player = new Player("Test Player");

            // Act
            player.HasBet = true;
            player.CurrentBid = 55;

            // Assert
            Assert.True(player.HasBet);
            Assert.Equal(55, player.CurrentBid);
        }

        [Fact]
        public void Player_PassedState_CanBeModified()
        {
            // Arrange
            var player = new Player("Test Player");

            // Act
            player.HasPassed = true;

            // Assert
            Assert.True(player.HasPassed);
        }
    }
}