using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Services;

namespace DeuxCentsCardGame.Tests.Services
{
    public class CardCollectionHelperTests
    {
        private readonly CardCollectionHelper _helper;

        public CardCollectionHelperTests()
        {
            _helper = new CardCollectionHelper();
        }

        private List<Card> CreateTestHand()
        {
            return new List<Card>
            {
                new Card(CardSuit.Hearts, CardFace.Ace, 10, 10),
                new Card(CardSuit.Hearts, CardFace.Five, 1, 5),
                new Card(CardSuit.Spades, CardFace.King, 9, 0),
                new Card(CardSuit.Clubs, CardFace.Ten, 6, 10),
                new Card(CardSuit.Diamonds, CardFace.Seven, 3, 0)
            };
        }

        #region FilterBySuit Tests

        [Fact]
        public void FilterBySuit_ReturnsOnlyCardsOfSpecifiedSuit()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.FilterBySuit(cards, CardSuit.Hearts);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, card => Assert.Equal(CardSuit.Hearts, card.CardSuit));
        }

        [Fact]
        public void FilterBySuit_ReturnsEmptyListWhenNoMatches()
        {
            // Arrange
            var cards = new List<Card>
            {
                new Card(CardSuit.Hearts, CardFace.Ace, 10, 10)
            };

            // Act
            var result = _helper.FilterBySuit(cards, CardSuit.Spades);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region FilterByTrump Tests

        [Fact]
        public void FilterByTrump_ReturnsOnlyTrumpCards()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.FilterByTrump(cards, CardSuit.Hearts);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, card => Assert.Equal(CardSuit.Hearts, card.CardSuit));
        }

        [Fact]
        public void FilterByTrump_ReturnsEmptyWhenNoTrumpSuit()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.FilterByTrump(cards, null);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region FilterByNonTrump Tests

        [Fact]
        public void FilterByNonTrump_ReturnsOnlyNonTrumpCards()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.FilterByNonTrump(cards, CardSuit.Hearts);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.All(result, card => Assert.NotEqual(CardSuit.Hearts, card.CardSuit));
        }

        [Fact]
        public void FilterByNonTrump_ReturnsAllCardsWhenNoTrumpSuit()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.FilterByNonTrump(cards, null);

            // Assert
            Assert.Equal(5, result.Count);
        }

        #endregion

        #region SortByValue Tests

        [Fact]
        public void SortByValue_SortsAscendingByDefault()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.SortByValue(cards);

            // Assert
            Assert.Equal(1, result[0].CardFaceValue);
            Assert.Equal(10, result[4].CardFaceValue);
        }

        [Fact]
        public void SortByValue_SortsDescendingWhenSpecified()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.SortByValue(cards, descending: true);

            // Assert
            Assert.Equal(10, result[0].CardFaceValue);
            Assert.Equal(1, result[4].CardFaceValue);
        }

        #endregion

        #region GetHighestCard Tests

        [Fact]
        public void GetHighestCard_ReturnsCardWithHighestValue()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.GetHighestCard(cards);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.CardFaceValue);
            Assert.Equal(CardFace.Ace, result.CardFace);
        }

        [Fact]
        public void GetHighestCard_ReturnsNullForEmptyList()
        {
            // Act
            var result = _helper.GetHighestCard(new List<Card>());

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetLowestCard Tests

        [Fact]
        public void GetLowestCard_ReturnsCardWithLowestValue()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.GetLowestCard(cards);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CardFaceValue);
            Assert.Equal(CardFace.Five, result.CardFace);
        }

        [Fact]
        public void GetLowestCard_ReturnsNullForEmptyList()
        {
            // Act
            var result = _helper.GetLowestCard(new List<Card>());

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GroupBySuit Tests

        [Fact]
        public void GroupBySuit_GroupsCardsCorrectly()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.GroupBySuit(cards);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal(2, result[CardSuit.Hearts].Count);
            Assert.Single(result[CardSuit.Spades]);
            Assert.Single(result[CardSuit.Clubs]);
            Assert.Single(result[CardSuit.Diamonds]);
        }

        #endregion

        #region CountBySuit Tests

        [Fact]
        public void CountBySuit_CountsCardsCorrectly()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.CountBySuit(cards);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal(2, result[CardSuit.Hearts]);
            Assert.Equal(1, result[CardSuit.Spades]);
            Assert.Equal(1, result[CardSuit.Clubs]);
            Assert.Equal(1, result[CardSuit.Diamonds]);
        }

        #endregion

        #region GetPointCards Tests

        [Fact]
        public void GetPointCards_ReturnsOnlyCardsWithPoints()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.GetPointCards(cards);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.All(result, card => Assert.True(card.CardPointValue > 0));
        }

        #endregion

        #region GetNonPointCards Tests

        [Fact]
        public void GetNonPointCards_ReturnsOnlyCardsWithoutPoints()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.GetNonPointCards(cards);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, card => Assert.Equal(0, card.CardPointValue));
        }

        #endregion

        #region CalculateTotalPoints Tests

        [Fact]
        public void CalculateTotalPoints_SumsCorrectly()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.CalculateTotalPoints(cards);

            // Assert
            Assert.Equal(25, result); // 10 + 5 + 0 + 10 + 0
        }

        [Fact]
        public void CalculateTotalPoints_ReturnsZeroForEmptyList()
        {
            // Act
            var result = _helper.CalculateTotalPoints(new List<Card>());

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region CalculateTotalFaceValue Tests

        [Fact]
        public void CalculateTotalFaceValue_SumsCorrectly()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.CalculateTotalFaceValue(cards);

            // Assert
            Assert.Equal(29, result); // 10 + 1 + 9 + 6 + 3
        }

        #endregion

        #region HasSuit Tests

        [Fact]
        public void HasSuit_ReturnsTrueWhenSuitExists()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.HasSuit(cards, CardSuit.Hearts);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasSuit_ReturnsFalseWhenSuitDoesNotExist()
        {
            // Arrange
            var cards = new List<Card>
            {
                new Card(CardSuit.Hearts, CardFace.Ace, 10, 10)
            };

            // Act
            var result = _helper.HasSuit(cards, CardSuit.Spades);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetPlayableCards Tests

        [Fact]
        public void GetPlayableCards_ReturnsAllCardsWhenNoLeadingSuit()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.GetPlayableCards(cards, null);

            // Assert
            Assert.Equal(5, result.Count);
        }

        [Fact]
        public void GetPlayableCards_ReturnsOnlyLeadingSuitWhenPresent()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.GetPlayableCards(cards, CardSuit.Hearts);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, card => Assert.Equal(CardSuit.Hearts, card.CardSuit));
        }

        [Fact]
        public void GetPlayableCards_ReturnsAllCardsWhenLeadingSuitNotPresent()
        {
            // Arrange
            var cards = new List<Card>
            {
                new Card(CardSuit.Hearts, CardFace.Ace, 10, 10),
                new Card(CardSuit.Spades, CardFace.King, 9, 0)
            };

            // Act
            var result = _helper.GetPlayableCards(cards, CardSuit.Clubs);

            // Assert
            Assert.Equal(2, result.Count);
        }

        #endregion

        #region GetMiddleCard Tests

        [Fact]
        public void GetMiddleCard_ReturnsMiddleValueCard()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.GetMiddleCard(cards);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.CardFaceValue);
        }

        [Fact]
        public void GetMiddleCard_ReturnsNullForEmptyList()
        {
            // Act
            var result = _helper.GetMiddleCard(new List<Card>());

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Clone Tests

        [Fact]
        public void Clone_CreatesNewList()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.Clone(cards);

            // Assert
            Assert.NotSame(cards, result);
            Assert.Equal(cards.Count, result.Count);
        }

        [Fact]
        public void Clone_ContainsSameCards()
        {
            // Arrange
            var cards = CreateTestHand();

            // Act
            var result = _helper.Clone(cards);

            // Assert
            for (int i = 0; i < cards.Count; i++)
            {
                Assert.Same(cards[i], result[i]);
            }
        }

        #endregion
    }
}