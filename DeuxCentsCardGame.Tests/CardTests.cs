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
            bool result = card.IsPlayableCard(leadingSuit, hand);

            // 3. Then or Assert
            Assert.True(result);
        }

        [Fact] // CanBePlayed test 2
        public void CanBePlayed_WhenCardMatchesLeadingSuit_ReturnTrue()
        {
            // 1. Given or Arrange
            CardSuit? leadingSuit = CardSuit.Hearts;
            var card = CreateCard(CardSuit.Hearts,CardFace.King);
            var hand = new List<Card>
            {
                card,
                CreateCard(CardSuit.Clubs,CardFace.King),
                CreateCard(CardSuit.Diamonds,CardFace.King),
                CreateCard(CardSuit.Spades,CardFace.King),
            };

            // 2. When or Act
            bool result = card.IsPlayableCard(leadingSuit, hand);

            // 3. Then or Assert
            Assert.True(result);
        }

        [Fact] // CanBePlayed test 3
        public void CanBePlayed_WhenCardDoesNotMatchLeadingSuitAndHandDoesNotHaveLeadingSuit_ReturnTrue()
        {
            // 1. Given or Arrange
            CardSuit? leadingSuit = CardSuit.Hearts;
            var card = CreateCard(CardSuit.Clubs,CardFace.Five);
            var hand = new List<Card>
            {
                card,
                CreateCard(CardSuit.Clubs,CardFace.King),
                CreateCard(CardSuit.Diamonds,CardFace.King),
                CreateCard(CardSuit.Spades,CardFace.King),
            };

            // 2. When or Act
            bool result = card.IsPlayableCard(leadingSuit, hand);

            // 3. Then or Assert
            Assert.True(result);
        }

        [Fact] // CanBePlayed test 4
        public void CanBePlayed_WhenCardDoesNotMatchLeadingSuitAndHandHasLeadingSuit_ReturnFalse()
        {
            // 1. Given or Arrange
            CardSuit? leadingSuit = CardSuit.Hearts;
            var card = CreateCard(CardSuit.Clubs,CardFace.Five);
            var hand = new List<Card>
            {
                card,
                CreateCard(CardSuit.Hearts,CardFace.King), // leading suit
                CreateCard(CardSuit.Clubs,CardFace.King),
                CreateCard(CardSuit.Diamonds,CardFace.King),
                CreateCard(CardSuit.Spades,CardFace.King),
            };

            // 2. When or Act
            bool result = card.IsPlayableCard(leadingSuit, hand);

            // 3. Then or Assert
            Assert.False(result);
        }

        [Fact] // CanBePlayed test 5
        public void CanBePlayed_WhenHandHasOnlyOneCardThatMatchesLeadingSuit_ReturnFalse()
        {
            // 1. Given or Arrange
            CardSuit? leadingSuit = CardSuit.Hearts;
            var card = CreateCard(CardSuit.Clubs,CardFace.Five);
            var hand = new List<Card>
            {
                card,
                CreateCard(CardSuit.Hearts,CardFace.King),
                CreateCard(CardSuit.Hearts,CardFace.Five),
                CreateCard(CardSuit.Hearts,CardFace.Ace),
                CreateCard(CardSuit.Hearts,CardFace.Jack)
            };

            // 2. When or Act
            bool result = card.IsPlayableCard(leadingSuit, hand);

            // 3. Then or Assert
            Assert.False(result);
        }

        [Fact]// CanBePlayed test 6
        public void CanBePlayed_WhenHandHasAllSuitsAndCardDoesNotMatchLeadingSuit_ReturnFalse()
        {
            // 1. Given or Arrange
            CardSuit? leadingSuit = CardSuit.Hearts;
            var card = CreateCard(CardSuit.Clubs,CardFace.Five);
            var hand = new List<Card>
            {
                card,
                CreateCard(CardSuit.Hearts,CardFace.King), // leading suit
                CreateCard(CardSuit.Clubs,CardFace.King),
                CreateCard(CardSuit.Diamonds,CardFace.King),
                CreateCard(CardSuit.Spades,CardFace.King),
            };

            // 2. When or Act
            bool result = card.IsPlayableCard(leadingSuit, hand);

            // 3. Then or Assert
            Assert.False(result);
        }

        [Theory] // CanBePlayed test 7
        [InlineData(CardSuit.Clubs)]
        [InlineData(CardSuit.Diamonds)]
        [InlineData(CardSuit.Hearts)]
        [InlineData(CardSuit.Spades)]
        public void CanBePlayed_WhenAllSuitsMatchLeadingSuit_ReturnTrue(CardSuit suit)
        {
            // 1. Given or Arrange
            CardSuit? leadingSuit = suit;
            var card = CreateCard(suit,CardFace.Five);
            var hand = new List<Card>
            {
                card,
                CreateCard(suit,CardFace.King),
            };

            // 2. When or Act
            bool result = card.IsPlayableCard(leadingSuit, hand);

            // 3. Then or Assert
            Assert.True(result);
        }

        [Fact] // Beats test 1
        public void Beats_WhenTrumpBeatsNonTrump_ReturnTrue()
        {
            // 1. Given or Arrange
            var trumpCard = CreateCard(CardSuit.Hearts, CardFace.Five);
            var nonTrumpCard = CreateCard(CardSuit.Clubs, CardFace.Ace);
            var trumpSuit = CardSuit.Hearts;

            // 2. When or Act
            bool result = trumpCard.WinsAgainst(nonTrumpCard, trumpSuit, null);

            // 3. Then or Assert
            Assert.True(result);
        }

        [Fact] // Beats test 2
        public void Beats_WhenNonTrumpVsTrump_ReturnFalse()
        {
            // 1. Given or Arrange
            var nonTrumpCard = CreateCard(CardSuit.Clubs, CardFace.Ace);
            var trumpCard = CreateCard(CardSuit.Hearts, CardFace.Five);
            var trumpSuit = CardSuit.Hearts;

            // 2. When or Act
            bool result = nonTrumpCard.WinsAgainst(trumpCard, trumpSuit, null);

            // 3. Then or Assert
            Assert.False(result);
        }

        [Fact] // Beats test 3
        public void Beats_WhenBothTrumpCards_HigherFaceValueWins()
        {
            // 1. Given or Arrange
            var highTrump = CreateCard(CardSuit.Hearts, CardFace.King);
            var lowTrump = CreateCard(CardSuit.Hearts, CardFace.Six);
            var trumpSuit = CardSuit.Hearts;

            // 2. When or Act
            bool result = highTrump.WinsAgainst(lowTrump, trumpSuit, null);

            // 3. Then or Assert
            Assert.True(result);
        }

        [Fact] // Beats test 4
        public void Beats_WhenBothTrumpCards_LowerFaceValueLoses()
        {
            // 1. Given or Arrange
            var lowTrump = CreateCard(CardSuit.Hearts, CardFace.Six);
            var highTrump = CreateCard(CardSuit.Hearts, CardFace.King);
            var trumpSuit = CardSuit.Hearts;

            // 2. When or Act
            bool result = lowTrump.WinsAgainst(highTrump, trumpSuit, null);

            // 3. Then or Assert
            Assert.False(result);
        }

        [Fact] // Beats test 5
        public void Beats_WhenSameSuitNonTrump_HigherFaceValueWins()
        {
            // 1. Given or Arrange
            var highCard = CreateCard(CardSuit.Clubs, CardFace.King);
            var lowCard = CreateCard(CardSuit.Clubs, CardFace.Seven);
            var trumpSuit = CardSuit.Hearts;

            // 2. When or Act
            bool result = highCard.WinsAgainst(lowCard, trumpSuit, null);

            // 3. Then or Assert
            Assert.True(result);
        }

        [Fact] // Beats test 6
        public void Beats_WhenLeadingSuitBeatsNonLeadingSuit_ReturnTrue()
        {
            // 1. Given or Arrange
            var leadingSuitCard = CreateCard(CardSuit.Spades, CardFace.Five);
            var nonLeadingSuitCard = CreateCard(CardSuit.Diamonds, CardFace.Ace);
            var trumpSuit = CardSuit.Hearts;
            var leadingSuit = CardSuit.Spades;

            // 2. When or Act
            bool result = leadingSuitCard.WinsAgainst(nonLeadingSuitCard, trumpSuit, leadingSuit);

            // 3. Then or Assert
            Assert.True(result);
        }

        [Fact] // Beats test 7
        public void Beats_WhenNonLeadingSuitVsLeadingSuit_ReturnFalse()
        {
            // 1. Given or Arrange
            var nonLeadingSuitCard = CreateCard(CardSuit.Diamonds, CardFace.Ace);
            var leadingSuitCard = CreateCard(CardSuit.Spades, CardFace.Five);
            var trumpSuit = CardSuit.Hearts;
            var leadingSuit = CardSuit.Spades;

            // 2. When or Act
            bool result = nonLeadingSuitCard.WinsAgainst(leadingSuitCard, trumpSuit, leadingSuit);

            // 3. Then or Assert
            Assert.False(result);
        }

        [Fact] // Beats test 8
        public void Beats_WhenNeitherMatchesLeadingSuit_FirstCardWins()
        {
            // 1. Given or Arrange
            var firstCard = CreateCard(CardSuit.Diamonds, CardFace.King);
            var secondCard = CreateCard(CardSuit.Clubs, CardFace.Ace);
            var trumpSuit = CardSuit.Hearts;
            var leadingSuit = CardSuit.Spades;

            // 2. When or Act - secondCard was played first, so firstCard should lose
            bool result = firstCard.WinsAgainst(secondCard, trumpSuit, leadingSuit);

            // 3. Then or Assert
            Assert.False(result);
        }

        [Fact] // Beats test 9
        public void Beats_WhenNoTrumpNoLeadingSuit_ReturnFalse()
        {
            // 1. Given or Arrange
            var card1 = CreateCard(CardSuit.Diamonds, CardFace.King);
            var card2 = CreateCard(CardSuit.Clubs, CardFace.Ace);

            // 2. When or Act
            bool result = card1.WinsAgainst(card2, null, null);

            // 3. Then or Assert
            Assert.False(result);
        }

        [Fact] // Beats test 10
        public void Beats_WhenTrumpOverridesLeadingSuit_ReturnTrue()
        {
            // 1. Given or Arrange
            var trumpCard = CreateCard(CardSuit.Hearts, CardFace.Five);
            var leadingSuitCard = CreateCard(CardSuit.Spades, CardFace.Ace);
            var trumpSuit = CardSuit.Hearts;
            var leadingSuit = CardSuit.Spades;

            // 2. When or Act
            bool result = trumpCard.WinsAgainst(leadingSuitCard, trumpSuit, leadingSuit);

            // 3. Then or Assert
            Assert.True(result);
        }

        [Fact] // Beats test 11
        public void Beats_WhenEqualFaceValueSameSuit_ReturnFalse()
        {
            // 1. Given or Arrange
            var card1 = CreateCard(CardSuit.Clubs, CardFace.Seven);
            var card2 = CreateCard(CardSuit.Clubs, CardFace.Seven);
            var trumpSuit = CardSuit.Hearts;

            // 2. When or Act
            bool result = card1.WinsAgainst(card2, trumpSuit, null);

            // 3. Then or Assert
            Assert.False(result);
        }
    }
}