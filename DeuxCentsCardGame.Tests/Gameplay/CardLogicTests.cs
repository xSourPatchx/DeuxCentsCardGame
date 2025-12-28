using DeuxCentsCardGame.Gameplay;
using DeuxCentsCardGame.Models;
using Xunit;

namespace DeuxCentsCardGame.Tests.Gameplay
{
    public class CardLogicTests
    {
        private readonly CardLogic _cardLogic;

        public CardLogicTests()
        {
            _cardLogic = new CardLogic();
        }

        #region Trump Card Tests

        [Fact]
        public void WinsAgainst_TrumpBeatsNonTrump()
        {
            // Arrange
            var trumpCard = new Card(CardSuit.Hearts, CardFace.Five, 1, 5);
            var nonTrumpCard = new Card(CardSuit.Spades, CardFace.Ace, 10, 10);
            var trumpSuit = CardSuit.Hearts;

            // Act
            var result = _cardLogic.WinsAgainst(trumpCard, nonTrumpCard, trumpSuit, null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void WinsAgainst_NonTrumpLosesToTrump()
        {
            // Arrange
            var nonTrumpCard = new Card(CardSuit.Spades, CardFace.Ace, 10, 10);
            var trumpCard = new Card(CardSuit.Hearts, CardFace.Five, 1, 5);
            var trumpSuit = CardSuit.Hearts;

            // Act
            var result = _cardLogic.WinsAgainst(nonTrumpCard, trumpCard, trumpSuit, null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WinsAgainst_HigherTrumpBeatsLowerTrump()
        {
            // Arrange
            var highTrump = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var lowTrump = new Card(CardSuit.Hearts, CardFace.Five, 1, 5);
            var trumpSuit = CardSuit.Hearts;

            // Act
            var result = _cardLogic.WinsAgainst(highTrump, lowTrump, trumpSuit, null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void WinsAgainst_LowerTrumpLosesToHigherTrump()
        {
            // Arrange
            var lowTrump = new Card(CardSuit.Hearts, CardFace.Five, 1, 5);
            var highTrump = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var trumpSuit = CardSuit.Hearts;

            // Act
            var result = _cardLogic.WinsAgainst(lowTrump, highTrump, trumpSuit, null);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Leading Suit Tests

        [Fact]
        public void WinsAgainst_LeadingSuitBeatsNonLeadingSuit()
        {
            // Arrange
            var leadingCard = new Card(CardSuit.Spades, CardFace.Five, 1, 5);
            var nonLeadingCard = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var leadingSuit = CardSuit.Spades;

            // Act
            var result = _cardLogic.WinsAgainst(leadingCard, nonLeadingCard, null, leadingSuit);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void WinsAgainst_NonLeadingSuitLosesToLeadingSuit()
        {
            // Arrange
            var nonLeadingCard = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var leadingCard = new Card(CardSuit.Spades, CardFace.Five, 1, 5);
            var leadingSuit = CardSuit.Spades;

            // Act
            var result = _cardLogic.WinsAgainst(nonLeadingCard, leadingCard, null, leadingSuit);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WinsAgainst_HigherLeadingSuitBeatsLowerLeadingSuit()
        {
            // Arrange
            var highCard = new Card(CardSuit.Spades, CardFace.Ace, 10, 10);
            var lowCard = new Card(CardSuit.Spades, CardFace.Five, 1, 5);
            var leadingSuit = CardSuit.Spades;

            // Act
            var result = _cardLogic.WinsAgainst(highCard, lowCard, null, leadingSuit);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Same Suit Tests

        [Fact]
        public void WinsAgainst_HigherValueWinsInSameSuit()
        {
            // Arrange
            var highCard = new Card(CardSuit.Clubs, CardFace.King, 9, 0);
            var lowCard = new Card(CardSuit.Clubs, CardFace.Six, 2, 0);

            // Act
            var result = _cardLogic.WinsAgainst(highCard, lowCard, null, null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void WinsAgainst_LowerValueLosesInSameSuit()
        {
            // Arrange
            var lowCard = new Card(CardSuit.Clubs, CardFace.Six, 2, 0);
            var highCard = new Card(CardSuit.Clubs, CardFace.King, 9, 0);

            // Act
            var result = _cardLogic.WinsAgainst(lowCard, highCard, null, null);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Different Suits Tests

        [Fact]
        public void WinsAgainst_DifferentSuitsNoLeadingNoTrump_ReturnsFalse()
        {
            // Arrange
            var card1 = new Card(CardSuit.Clubs, CardFace.Ace, 10, 10);
            var card2 = new Card(CardSuit.Hearts, CardFace.Five, 1, 5);

            // Act
            var result = _cardLogic.WinsAgainst(card1, card2, null, null);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Complex Scenarios

        [Fact]
        public void WinsAgainst_TrumpBeatsLeadingSuit()
        {
            // Arrange
            var trumpCard = new Card(CardSuit.Hearts, CardFace.Five, 1, 5);
            var leadingCard = new Card(CardSuit.Spades, CardFace.Ace, 10, 10);
            var trumpSuit = CardSuit.Hearts;
            var leadingSuit = CardSuit.Spades;

            // Act
            var result = _cardLogic.WinsAgainst(trumpCard, leadingCard, trumpSuit, leadingSuit);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void WinsAgainst_LeadingSuitLosesToTrump()
        {
            // Arrange
            var leadingCard = new Card(CardSuit.Spades, CardFace.Ace, 10, 10);
            var trumpCard = new Card(CardSuit.Hearts, CardFace.Five, 1, 5);
            var trumpSuit = CardSuit.Hearts;
            var leadingSuit = CardSuit.Spades;

            // Act
            var result = _cardLogic.WinsAgainst(leadingCard, trumpCard, trumpSuit, leadingSuit);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WinsAgainst_TrumpAndLeadingSuitBothCards_HigherValueWins()
        {
            // Arrange - both cards are trump AND leading suit
            var highCard = new Card(CardSuit.Hearts, CardFace.Ace, 10, 10);
            var lowCard = new Card(CardSuit.Hearts, CardFace.Five, 1, 5);
            var trumpSuit = CardSuit.Hearts;
            var leadingSuit = CardSuit.Hearts;

            // Act
            var result = _cardLogic.WinsAgainst(highCard, lowCard, trumpSuit, leadingSuit);

            // Assert
            Assert.True(result);
        }

        #endregion
    }
}