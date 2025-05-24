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

            // 2. When or Act
            bool result = card.IsTrump(trumpSuit);

            // 3. Then or Assert
            Assert.True(result);
        }

        [Fact] // test 2
        public void IsTrump_WhenCardSuitDoesNotMatchTrumpSuit_ReturnFalse()
        {
            // 1. Given or Arrange
            var card = new Card(CardSuit.Hearts,CardFace.King, 9, 0);
            CardSuit trumpSuit = CardSuit.Clubs;

            // 2. When or Act
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

            // 2. When or Act
            bool result = card.IsTrump(trumpSuit);

            // 3. Then or Assert
            Assert.False(result);
        }

        [Fact] // test 4
        public void IsSameSuit_WhenCardIsSameSuitAsOtherCard_ReturnTrue()
        {
            // 1. Given or Arrange
            var card = new Card(CardSuit.Hearts,CardFace.King, 9, 0);
            var otherCard = new Card(CardSuit.Hearts,CardFace.Ace, 10, 10);

            // 2. When or Act
            bool result = card.IsSameSuit(otherCard);

            // 3. Then or Assert
            Assert.True(result);
        }

        [Fact] // test 5
        public void IsSameSuit_WhenCardIsNotSameSuitAsOtherCard_ReturnFalse()
        {
            // 1. Given or Arrange
            var card = new Card(CardSuit.Hearts,CardFace.King, 9, 0);
            var otherCard = new Card(CardSuit.Clubs,CardFace.King, 9, 0);

            // 2. When or Act
            bool result = card.IsSameSuit(otherCard);

            // 3. Then or Assert
            Assert.False(result);
        }

        [Fact] // test 5
        public void IsSameSuit_WhenCardsSuitAllDefer_ReturnAccordingly()
        {
            // 1. Given or Arrange
            var card = new Card(CardSuit.Clubs,CardFace.King, 9, 0);
            var diamondsCard = new Card(CardSuit.Diamonds,CardFace.King, 9, 0);
            var heartsCard = new Card(CardSuit.Hearts,CardFace.King, 9, 0);
            var spadesCard = new Card(CardSuit.Spades,CardFace.King, 9, 0);

            // 2. When or Act
            bool result1 = card.IsSameSuit(card);
            bool result2 = card.IsSameSuit(diamondsCard);
            bool result3 = card.IsSameSuit(heartsCard);
            bool result4 = card.IsSameSuit(spadesCard);

            // 3. Then or Assert
            Assert.True(result1); // same card
            Assert.False(result2);
            Assert.False(result3);
            Assert.False(result4);
        }
    }
}