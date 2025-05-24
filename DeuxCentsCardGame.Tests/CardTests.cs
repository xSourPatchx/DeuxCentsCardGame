// using Xunit;
// using DeuxCentsCardGame;

namespace DeuxCentsCardGame.Tests
{
    public class CardTests
    {
        [Fact] // test 1
        public void IsTrump_WhenCardSuitMatchTrumpSuit_ReturnTrue()
        {
            // 1. Given or Arrange
            var card = new Card(CardSuit.Hearts,CardFace.King, 9, 0);
            CardSuit trumpSuit = CardSuit.Hearts;

            // 2. When or Act - Call the private method using reflection
            bool ResultCardSuitMatch = card.IsTrump(trumpSuit);

            // 3. Then or Assert
            Assert.True(ResultCardSuitMatch);
        }

        [Fact] // test 2
        public void IsTrump_WhenCardSuitDoesNotMatchTrumpSuit_ReturnFalse()
        {
            // 1. Given or Arrange
            var card = new Card(CardSuit.Hearts,CardFace.King, 9, 0);
            CardSuit trumpSuit = CardSuit.Clubs;

            // 2. When or Act - Call the private method using reflection
            bool result = card.IsTrump(trumpSuit);

            // 3. Then or Assert
            Assert.False(result);
        }

        [Fact] // test 3
        public void IsTrump_WhenTrumpSuitIsNull_ReturnFalse()
        {
            // 1. Given or Arrange
            var card = new Card(CardSuit.Hearts,CardFace.King, 9, 0);
            CardSuit? trumpSuit = null;

            // 2. When or Act - Call the private method using reflection
            bool result = card.IsTrump(trumpSuit);

            // 3. Then or Assert
            Assert.False(result);
        }

    }
}