// using Xunit;
// using DeuxCentsCardGame;

namespace DeuxCentsCardGame.Tests
{
    public class CardTests
    {
        // Helper method to quickly create a card
        private Card CreateCard(CardSuit suit, CardFace face)
        {
            int pointValue;
            if (face == CardFace.Five)
            {
                pointValue = 5;
            }
            else if (face == CardFace.Ten || face == CardFace.Ace)
            {
                pointValue = 10;
            }
            else
            {
                pointValue = 0;
            }
            
            int faceValue = (int)face +1; // enum to auto assign faceValue

            return new Card(suit, face, faceValue, pointValue);
        }


        [Fact] // IsTrump test 1
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

        [Fact] // IsTrump test 2
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

        [Fact] // IsTrump test 3
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

        [Fact] // IsSameSuit test 1
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

        [Fact] // IsSameSuit test 2
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

        [Fact] // IsSameSuit test 3
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

        [Fact] // CanBePlayed test 1
        public void CanBePlayed_WhenNoLeadingSuit_ReturnTrue()
        {
            // 1. Given or Arrange
            CardSuit? leadingSuit = null;
            var card = CreateCard(CardSuit.Hearts,CardFace.King);
            var hand = new List<Card>
            {
                CreateCard(CardSuit.Clubs,CardFace.King),
                CreateCard(CardSuit.Diamonds,CardFace.King),
                CreateCard(CardSuit.Spades,CardFace.King),
            };

            // 2. When or Act
            bool result = card.CanBePlayed(leadingSuit, hand);

            // 3. Then or Assert
            Assert.True(result);
        }

        // CanBePlayed test 2
        // CanBePlayed test 3
        // CanBePlayed test 4
        // CanBePlayed test 5
        // CanBePlayed test 6
    }
}