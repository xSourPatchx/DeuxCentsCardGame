using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void Constructor_ShouldInitializePlayerWithName()
        {
            // Arrange
            var playerName = "TestPlayer";

            // Act
            var player = new Player(playerName);

            // Assert
            Assert.Equal(playerName, player.Name);
        }

        [Fact]
        public void Constructor_ShouldInitializeEmptyHand()
        {
            // Arrange & Act
            var player = new Player("TestPlayer");

            // Assert
            Assert.NotNull(player.Hand);
            Assert.Empty(player.Hand);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Alice")]
        [InlineData("Player123")]
        [InlineData("VeryLongPlayerNameWithSpecialCharacters!@#$")]
        public void Constructor_ShouldAcceptVariousNameFormats(string name)
        {
            // Arrange & Act
            var player = new Player(name);

            // Assert
            Assert.Equal(name, player.Name);
        }

        [Fact]
        public void Name_PropertyShouldBeReadOnly()
        {
            // This test ensures the Name property setter is not public
            var nameProperty = typeof(Player).GetProperty("Name");
            
            Assert.NotNull(nameProperty);
            Assert.True(nameProperty.CanRead);
            Assert.False(nameProperty.SetMethod?.IsPublic ?? false);
        }

        [Fact]
        public void Hand_PropertyShouldBeReadOnly()
        {
            // This test ensures the Hand property setter is not public
            var handProperty = typeof(Player).GetProperty("Hand");
            
            Assert.NotNull(handProperty);
            Assert.True(handProperty.CanRead);
            Assert.False(handProperty.SetMethod?.IsPublic ?? false);
        }

        [Fact]
        public void AddCard_ShouldAddCardToHand()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);

            // Act
            player.AddCard(card);

            // Assert
            Assert.Single(player.Hand);
            Assert.Contains(card, player.Hand);
        }

        [Fact]
        public void AddCard_ShouldAddMultipleCards()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Spades, CardFace.King, 9, 0);
            var card3 = new Card(CardSuit.Clubs, CardFace.Five, 1, 5);

            // Act
            player.AddCard(card1);
            player.AddCard(card2);
            player.AddCard(card3);

            // Assert
            Assert.Equal(3, player.Hand.Count);
            Assert.Contains(card1, player.Hand);
            Assert.Contains(card2, player.Hand);
            Assert.Contains(card3, player.Hand);
        }

        [Fact]
        public void AddCard_ShouldAllowDuplicateCards()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10); // Same values as card1

            // Act
            player.AddCard(card1);
            player.AddCard(card2);

            // Assert
            Assert.Equal(2, player.Hand.Count);
            Assert.Contains(card1, player.Hand);
            Assert.Contains(card2, player.Hand);
        }

        [Fact]
        public void AddCard_WithNullCard_ShouldAddNullToHand()
        {
            // Arrange
            var player = new Player("TestPlayer");

            // Act
            player.AddCard(null);

            // Assert
            Assert.Single(player.Hand);
            Assert.Contains(null, player.Hand);
        }

        [Fact]
        public void RemoveCard_ShouldRemoveCardFromHand()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            player.AddCard(card);

            // Act
            player.RemoveCard(card);

            // Assert
            Assert.Empty(player.Hand);
            Assert.DoesNotContain(card, player.Hand);
        }

        [Fact]
        public void RemoveCard_ShouldRemoveOnlyFirstMatchingCard()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10); // Same values as card1
            var card3 = new Card(CardSuit.Spades, CardFace.King, 9, 0);
            
            player.AddCard(card1);
            player.AddCard(card2);
            player.AddCard(card3);

            // Act
            player.RemoveCard(card1);

            // Assert
            Assert.Equal(2, player.Hand.Count);
            Assert.DoesNotContain(card1, player.Hand);
            Assert.Contains(card2, player.Hand);
            Assert.Contains(card3, player.Hand);
        }

        [Fact]
        public void RemoveCard_WithCardNotInHand_ShouldNotChangeHand()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var cardInHand = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var cardNotInHand = new Card(CardSuit.Spades, CardFace.King, 9, 0);
            player.AddCard(cardInHand);

            // Act
            player.RemoveCard(cardNotInHand);

            // Assert
            Assert.Single(player.Hand);
            Assert.Contains(cardInHand, player.Hand);
        }

        [Fact]
        public void RemoveCard_WithNullCard_ShouldRemoveNullFromHand()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            player.AddCard(card);
            player.AddCard(null);

            // Act
            player.RemoveCard(null);

            // Assert
            Assert.Single(player.Hand);
            Assert.Contains(card, player.Hand);
            Assert.DoesNotContain(null, player.Hand);
        }

        [Fact]
        public void RemoveCard_FromEmptyHand_ShouldNotThrowException()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);

            // Act & Assert
            var exception = Record.Exception(() => player.RemoveCard(card));
            Assert.Null(exception);
            Assert.Empty(player.Hand);
        }

        [Fact]
        public void Hand_ShouldMaintainCardOrder()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var card1 = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Spades, CardFace.King, 9, 0);
            var card3 = new Card(CardSuit.Clubs, CardFace.Five, 1, 5);

            // Act
            player.AddCard(card1);
            player.AddCard(card2);
            player.AddCard(card3);

            // Assert
            Assert.Equal(card1, player.Hand[0]);
            Assert.Equal(card2, player.Hand[1]);
            Assert.Equal(card3, player.Hand[2]);
        }

        [Fact]
        public void Player_ShouldHandleComplexCardManipulation()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var deck = new Deck();
            var cardsToAdd = deck.Cards.Take(5).ToList();

            // Act - Add multiple cards
            foreach (var card in cardsToAdd)
            {
                player.AddCard(card);
            }

            // Remove middle card
            var middleCard = cardsToAdd[2];
            player.RemoveCard(middleCard);

            // Add another card
            var newCard = deck.Cards.Skip(5).First();
            player.AddCard(newCard);

            // Assert
            Assert.Equal(5, player.Hand.Count);
            Assert.DoesNotContain(middleCard, player.Hand);
            Assert.Contains(newCard, player.Hand);
            Assert.Contains(cardsToAdd[0], player.Hand);
            Assert.Contains(cardsToAdd[1], player.Hand);
            Assert.Contains(cardsToAdd[3], player.Hand);
            Assert.Contains(cardsToAdd[4], player.Hand);
        }

        [Fact]
        public void Player_ShouldSupportLargeHandSize()
        {
            // Arrange
            var player = new Player("TestPlayer");
            var deck = new Deck();

            // Act - Add all cards from deck
            foreach (var card in deck.Cards)
            {
                player.AddCard(card);
            }

            // Assert
            Assert.Equal(40, player.Hand.Count);
            Assert.True(player.Hand.All(c => deck.Cards.Contains(c)));
        }
    }
}