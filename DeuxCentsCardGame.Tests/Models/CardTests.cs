using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Models
{
    public class CardTests
    {
        [Theory]
        [InlineData(CardSuit.Hearts, CardFace.Ace, 10, 10)]
        [InlineData(CardSuit.Spades, CardFace.King, 9, 0)]
        [InlineData(CardSuit.Diamonds, CardFace.Ten, 5, 10)]
        [InlineData(CardSuit.Clubs, CardFace.Five, 1, 5)]
        public void Card_ValidConstructor_CreatesSuccessfully(CardSuit suit, CardFace face, int faceValue, int pointValue)
        {
            // Act & Assert - Should not throw
            var card = new Card(suit, face, faceValue, pointValue);
            
            Assert.Equal(suit, card.CardSuit);
            Assert.Equal(face, card.CardFace);
            Assert.Equal(faceValue, card.CardFaceValue);
            Assert.Equal(pointValue, card.CardPointValue);
        }

        [Fact]
        public void Card_InvalidPointValue_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Card(CardSuit.Hearts, CardFace.Ace, 10, 15)); // 15 is invalid point value
        }

        [Fact]
        public void IsPlayableCard_NoLeadingSuit_ReturnsTrue()
        {
            // Arrange
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var hand = new List<Card> { card };

            // Act
            bool result = card.IsPlayableCard(null, hand);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPlayableCard_MatchingLeadingSuit_ReturnsTrue()
        {
            // Arrange
            var card = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var hand = new List<Card> { card };

            // Act
            bool result = card.IsPlayableCard(CardSuit.Hearts, hand);

            // Assert
            Assert.True(result);
        }
    }
}