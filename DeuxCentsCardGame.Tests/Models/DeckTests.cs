using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Models
{
    public class DeckTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_InitializesEmptyCardsList()
        {
            // Arrange & Act
            var deck = new Deck();

            // Assert
            Assert.NotNull(deck.Cards);
        }

        [Fact]
        public void Constructor_Creates40Cards()
        {
            // Arrange & Act
            var deck = new Deck();

            // Assert
            Assert.Equal(40, deck.Cards.Count); // 4 suits × 10 faces = 40 cards
        }

        [Fact]
        public void Constructor_CreatesAllSuitCombinations()
        {
            // Arrange & Act
            var deck = new Deck();
            var expectedSuits = new[] { CardSuit.Clubs, CardSuit.Diamonds, CardSuit.Hearts, CardSuit.Spades };

            // Assert
            foreach (var suit in expectedSuits)
            {
                var cardsOfSuit = deck.Cards.Where(c => c.CardSuit == suit).ToList();
                Assert.Equal(10, cardsOfSuit.Count);
            }
        }

        [Fact]
        public void Constructor_CreatesAllFaceCombinations()
        {
            // Arrange & Act
            var deck = new Deck();
            var expectedFaces = new[] 
            { 
                CardFace.Five, CardFace.Six, CardFace.Seven, CardFace.Eight, CardFace.Nine,
                CardFace.Ten, CardFace.Jack, CardFace.Queen, CardFace.King, CardFace.Ace 
            };

            // Assert
            foreach (var face in expectedFaces)
            {
                var cardsOfFace = deck.Cards.Where(c => c.CardFace == face).ToList();
                Assert.Equal(4, cardsOfFace.Count); // One for each suit
            }
        }

        [Fact]
        public void Constructor_CreatesUniqueSuitFaceCombinations()
        {
            // Arrange & Act
            var deck = new Deck();
            var expectedSuits = new[] { CardSuit.Clubs, CardSuit.Diamonds, CardSuit.Hearts, CardSuit.Spades };
            var expectedFaces = new[] 
            { 
                CardFace.Five, CardFace.Six, CardFace.Seven, CardFace.Eight, CardFace.Nine,
                CardFace.Ten, CardFace.Jack, CardFace.Queen, CardFace.King, CardFace.Ace 
            };

            // Assert
            foreach (var suit in expectedSuits)
            {
                foreach (var face in expectedFaces)
                {
                    var matchingCards = deck.Cards.Where(c => c.CardSuit == suit && c.CardFace == face).ToList();
                    Assert.Single(matchingCards);
                }
            }
        }

        #endregion

        #region Card Values Tests

        [Theory]
        [InlineData(CardFace.Five, 1)]
        [InlineData(CardFace.Six, 2)]
        [InlineData(CardFace.Seven, 3)]
        [InlineData(CardFace.Eight, 4)]
        [InlineData(CardFace.Nine, 5)]
        [InlineData(CardFace.Ten, 6)]
        [InlineData(CardFace.Jack, 7)]
        [InlineData(CardFace.Queen, 8)]
        [InlineData(CardFace.King, 9)]
        [InlineData(CardFace.Ace, 10)]
        public void Constructor_AssignsCorrectFaceValues(CardFace face, int expectedValue)
        {
            // Arrange & Act
            var deck = new Deck();

            // Assert
            var cardsWithFace = deck.Cards.Where(c => c.CardFace == face).ToList();
            Assert.All(cardsWithFace, card => Assert.Equal(expectedValue, card.CardFaceValue));
        }

        [Theory]
        [InlineData(CardFace.Five, 5)]
        [InlineData(CardFace.Six, 0)]
        [InlineData(CardFace.Seven, 0)]
        [InlineData(CardFace.Eight, 0)]
        [InlineData(CardFace.Nine, 0)]
        [InlineData(CardFace.Ten, 10)]
        [InlineData(CardFace.Jack, 0)]
        [InlineData(CardFace.Queen, 0)]
        [InlineData(CardFace.King, 0)]
        [InlineData(CardFace.Ace, 10)]
        public void Constructor_AssignsCorrectPointValues(CardFace face, int expectedPoints)
        {
            // Arrange & Act
            var deck = new Deck();

            // Assert
            var cardsWithFace = deck.Cards.Where(c => c.CardFace == face).ToList();
            Assert.All(cardsWithFace, card => Assert.Equal(expectedPoints, card.CardPointValue));
        }

        [Fact]
        public void Deck_TotalPointValueIs100()
        {
            // Arrange & Act
            var deck = new Deck();
            var totalPoints = deck.Cards.Sum(c => c.CardPointValue);

            // Assert
            // 4 suits × (5 + 10 + 10) = 4 × 25 = 100
            Assert.Equal(100, totalPoints);
        }

        [Fact]
        public void Deck_HasCorrectNumberOfPointCards()
        {
            // Arrange & Act
            var deck = new Deck();

            // Assert
            var fives = deck.Cards.Count(c => c.CardFace == CardFace.Five);
            var tens = deck.Cards.Count(c => c.CardFace == CardFace.Ten);
            var aces = deck.Cards.Count(c => c.CardFace == CardFace.Ace);

            Assert.Equal(4, fives);
            Assert.Equal(4, tens);
            Assert.Equal(4, aces);
        }

        #endregion

        #region GetCardSuits Tests

        [Fact]
        public void GetCardSuits_ReturnsAllFourSuits()
        {
            // Arrange & Act
            var suits = Deck.GetCardSuits();

            // Assert
            Assert.Equal(4, suits.Length);
            Assert.Contains(CardSuit.Clubs, suits);
            Assert.Contains(CardSuit.Diamonds, suits);
            Assert.Contains(CardSuit.Hearts, suits);
            Assert.Contains(CardSuit.Spades, suits);
        }

        [Fact]
        public void GetCardSuits_ReturnsSameArrayEachTime()
        {
            // Arrange & Act
            var suits1 = Deck.GetCardSuits();
            var suits2 = Deck.GetCardSuits();

            // Assert
            Assert.Equal(suits1, suits2);
        }

        #endregion

        #region StringToCardSuit Tests

        [Theory]
        [InlineData("clubs", CardSuit.Clubs)]
        [InlineData("diamonds", CardSuit.Diamonds)]
        [InlineData("hearts", CardSuit.Hearts)]
        [InlineData("spades", CardSuit.Spades)]
        public void StringToCardSuit_WithLowercase_ReturnsCorrectSuit(string input, CardSuit expected)
        {
            // Arrange & Act
            var result = Deck.StringToCardSuit(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("CLUBS", CardSuit.Clubs)]
        [InlineData("DIAMONDS", CardSuit.Diamonds)]
        [InlineData("HEARTS", CardSuit.Hearts)]
        [InlineData("SPADES", CardSuit.Spades)]
        public void StringToCardSuit_WithUppercase_ReturnsCorrectSuit(string input, CardSuit expected)
        {
            // Arrange & Act
            var result = Deck.StringToCardSuit(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Clubs", CardSuit.Clubs)]
        [InlineData("Diamonds", CardSuit.Diamonds)]
        [InlineData("Hearts", CardSuit.Hearts)]
        [InlineData("Spades", CardSuit.Spades)]
        public void StringToCardSuit_WithMixedCase_ReturnsCorrectSuit(string input, CardSuit expected)
        {
            // Arrange & Act
            var result = Deck.StringToCardSuit(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("club")]
        [InlineData("diamond")]
        [InlineData("heart")]
        [InlineData("spade")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("123")]
        public void StringToCardSuit_WithInvalidInput_ThrowsArgumentException(string input)
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Deck.StringToCardSuit(input));
            Assert.Contains($"Invalid suit name: {input}", exception.Message);
        }

        #endregion

        #region CardSuitToString Tests

        [Theory]
        [InlineData(CardSuit.Clubs, "clubs")]
        [InlineData(CardSuit.Diamonds, "diamonds")]
        [InlineData(CardSuit.Hearts, "hearts")]
        [InlineData(CardSuit.Spades, "spades")]
        public void CardSuitToString_ReturnsLowercaseSuitName(CardSuit suit, string expected)
        {
            // Arrange & Act
            var result = Deck.CardSuitToString(suit);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Cards_PropertyIsReadable()
        {
            // Arrange
            var deck = new Deck();

            // Act & Assert
            Assert.NotNull(deck.Cards);
            var cardsProperty = typeof(Deck).GetProperty("Cards");
            Assert.NotNull(cardsProperty);
            Assert.True(cardsProperty.CanRead);
        }

        [Fact]
        public void Cards_PropertySetterIsPrivate()
        {
            // Arrange
            var cardsProperty = typeof(Deck).GetProperty("Cards");

            // Act & Assert
            Assert.NotNull(cardsProperty);
            Assert.False(cardsProperty.SetMethod?.IsPublic ?? false);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Deck_CanBeUsedToDistributeCardsToPlayers()
        {
            // Arrange
            var deck = new Deck();
            var player1 = new Player("Player 1");
            var player2 = new Player("Player 2");

            // Act - Deal 5 cards to each player
            for (int i = 0; i < 5; i++)
            {
                player1.AddCard(deck.Cards[i * 2]);
                player2.AddCard(deck.Cards[i * 2 + 1]);
            }

            // Assert
            Assert.Equal(5, player1.Hand.Count);
            Assert.Equal(5, player2.Hand.Count);
            Assert.Equal(40, deck.Cards.Count); // Deck still has all cards
        }

        [Fact]
        public void MultipleDeckInstances_AreIndependent()
        {
            // Arrange & Act
            var deck1 = new Deck();
            var deck2 = new Deck();

            // Assert
            Assert.Equal(deck1.Cards.Count, deck2.Cards.Count);
            Assert.NotSame(deck1.Cards, deck2.Cards);
        }

        #endregion
    }
}