using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Models
{
    public class CardTests
    {
        [Fact]
        public void Card_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);

            // Assert
            Assert.Equal(CardSuit.Hearts, card.CardSuit);
            Assert.Equal(CardFace.Ace, card.CardFace);
            Assert.Equal(10, card.CardFaceValue);
            Assert.Equal(10, card.CardPointValue);
        }

        [Fact]
        public void IsTrump_WhenSuitMatchesTrumpSuit_ReturnsTrue()
        {
            // Arrange
            var card = new Card(CardSuit.Spades, CardFace.King, 9, 0);
            var trumpSuit = CardSuit.Spades;

            // Act
            var result = card.IsTrump(trumpSuit);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsTrump_WhenSuitDoesNotMatchTrumpSuit_ReturnsFalse()
        {
            // Arrange
            var card = new Card(CardSuit.Hearts, CardFace.King, 9, 0);
            var trumpSuit = CardSuit.Spades;

            // Act
            var result = card.IsTrump(trumpSuit);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsTrump_WhenTrumpSuitIsNull_ReturnsFalse()
        {
            // Arrange
            var card = new Card(CardSuit.Hearts, CardFace.King, 9, 0);

            // Act
            var result = card.IsTrump(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsSameSuit_WhenSuitsMatch_ReturnsTrue()
        {
            // Arrange
            var card1 = new Card(CardSuit.Clubs, CardFace.Five, 1, 5);
            var card2 = new Card(CardSuit.Clubs, CardFace.Ten, 6, 10);

            // Act
            var result = card1.IsSameSuit(card2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSameSuit_WhenSuitsDontMatch_ReturnsFalse()
        {
            // Arrange
            var card1 = new Card(CardSuit.Clubs, CardFace.Five, 1, 5);
            var card2 = new Card(CardSuit.Diamonds, CardFace.Ten, 6, 10);

            // Act
            var result = card1.IsSameSuit(card2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);

            // Act
            var result = card.ToString();

            // Assert
            Assert.Equal("Ace of hearts (10 pts)", result);
        }

        [Theory]
        [InlineData(CardSuit.Clubs, CardFace.Five, 1, 5)]
        [InlineData(CardSuit.Diamonds, CardFace.Ten, 6, 10)]
        [InlineData(CardSuit.Hearts, CardFace.Ace, 10, 10)]
        [InlineData(CardSuit.Spades, CardFace.Jack, 7, 0)]
        public void Card_CreatesValidCards_WithVariousValues(
            CardSuit suit, CardFace face, int faceValue, int pointValue)
        {
            // Act
            var card = new Card(suit, face, faceValue, pointValue);

            // Assert
            Assert.Equal(suit, card.CardSuit);
            Assert.Equal(face, card.CardFace);
            Assert.Equal(faceValue, card.CardFaceValue);
            Assert.Equal(pointValue, card.CardPointValue);
        }
    }
}