namespace DeuxCentsCardGame.Tests
{
    public class DeckTests
    {
        [Fact]
        public void Constructor_ShouldInitializeDeckWith40Cards()
        {
            // Arrange & Act
            var deck = new Deck();

            // Assert
            Assert.NotNull(deck.Cards);
            Assert.Equal(40, deck.Cards.Count); // 4 suits × 10 faces = 40 cards
        }

        [Fact]
        public void Constructor_ShouldCreateCardsWithCorrectSuitsAndFaces()
        {
            // Arrange & Act
            var deck = new Deck();

            // Assert
            var expectedSuits = new[] { CardSuit.Clubs, CardSuit.Diamonds, CardSuit.Hearts, CardSuit.Spades };
            var expectedFaces = new[] { CardFace.Five, CardFace.Six, CardFace.Seven, CardFace.Eight, CardFace.Nine, 
                                      CardFace.Ten, CardFace.Jack, CardFace.Queen, CardFace.King, CardFace.Ace };

            // Check that all suits are represented
            foreach (var suit in expectedSuits)
            {
                var cardsOfSuit = deck.Cards.Where(c => c.CardSuit == suit).ToList();
                Assert.Equal(10, cardsOfSuit.Count);
            }

            // Check that all faces are represented for each suit
            foreach (var suit in expectedSuits)
            {
                foreach (var face in expectedFaces)
                {
                    Assert.Single(deck.Cards.Where(c => c.CardSuit == suit && c.CardFace == face));
                }
            }
        }

        [Fact]
        public void Constructor_ShouldAssignCorrectFaceValues()
        {
            // Arrange & Act
            var deck = new Deck();

            // Assert
            var expectedFaceValues = new Dictionary<CardFace, int>
            {
                { CardFace.Five, 1 },
                { CardFace.Six, 2 },
                { CardFace.Seven, 3 },
                { CardFace.Eight, 4 },
                { CardFace.Nine, 5 },
                { CardFace.Ten, 6 },
                { CardFace.Jack, 7 },
                { CardFace.Queen, 8 },
                { CardFace.King, 9 },
                { CardFace.Ace, 10 }
            };

            foreach (var card in deck.Cards)
            {
                Assert.Equal(expectedFaceValues[card.CardFace], card.CardFaceValue);
            }
        }

        [Fact]
        public void Constructor_ShouldAssignCorrectPointValues()
        {
            // Arrange & Act
            var deck = new Deck();

            // Assert
            var expectedPointValues = new Dictionary<CardFace, int>
            {
                { CardFace.Five, 5 },
                { CardFace.Six, 0 },
                { CardFace.Seven, 0 },
                { CardFace.Eight, 0 },
                { CardFace.Nine, 0 },
                { CardFace.Ten, 10 },
                { CardFace.Jack, 0 },
                { CardFace.Queen, 0 },
                { CardFace.King, 0 },
                { CardFace.Ace, 10 }
            };

            foreach (var card in deck.Cards)
            {
                Assert.Equal(expectedPointValues[card.CardFace], card.CardPointValue);
            }
        }

        [Fact]
        public void ShuffleDeck_ShouldChangeCardOrder()
        {
            // Arrange
            var deck = new Deck();
            var originalOrder = deck.Cards.ToList(); // Create a copy of the original order

            // Act
            deck.ShuffleDeck();

            // Assert
            // While it's theoretically possible for shuffle to result in the same order,
            // it's extremely unlikely with 40 cards, so this test should be reliable
            Assert.NotEqual(originalOrder, deck.Cards);
        }

        [Fact]
        public void ShuffleDeck_ShouldMaintainSameCards()
        {
            // Arrange
            var deck = new Deck();
            var originalCards = deck.Cards.ToHashSet();

            // Act
            deck.ShuffleDeck();

            // Assert
            var shuffledCards = deck.Cards.ToHashSet();
            Assert.Equal(originalCards.Count, shuffledCards.Count);
            Assert.True(originalCards.SetEquals(shuffledCards));
        }

        [Fact]
        public void ShuffleDeck_ShouldMaintainCardCount()
        {
            // Arrange
            var deck = new Deck();
            var originalCount = deck.Cards.Count;

            // Act
            deck.ShuffleDeck();

            // Assert
            Assert.Equal(originalCount, deck.Cards.Count);
        }

        [Fact]
        public void GetCardSuits_ShouldReturnAllFourSuits()
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

        [Theory]
        [InlineData("clubs", CardSuit.Clubs)]
        [InlineData("diamonds", CardSuit.Diamonds)]
        [InlineData("hearts", CardSuit.Hearts)]
        [InlineData("spades", CardSuit.Spades)]
        [InlineData("CLUBS", CardSuit.Clubs)]
        [InlineData("Diamonds", CardSuit.Diamonds)]
        [InlineData("HEARTS", CardSuit.Hearts)]
        [InlineData("SpAdEs", CardSuit.Spades)]
        public void StringToCardSuit_ShouldReturnCorrectSuit_ForValidInput(string input, CardSuit expected)
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
        public void StringToCardSuit_ShouldThrowArgumentException_ForInvalidInput(string input)
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Deck.StringToCardSuit(input));
            Assert.Contains($"Invalid suit name: {input}", exception.Message);
        }

        [Theory]
        [InlineData(CardSuit.Clubs, "clubs")]
        [InlineData(CardSuit.Diamonds, "diamonds")]
        [InlineData(CardSuit.Hearts, "hearts")]
        [InlineData(CardSuit.Spades, "spades")]
        public void CardSuitToString_ShouldReturnCorrectString_ForValidSuit(CardSuit suit, string expected)
        {
            // Arrange & Act
            var result = Deck.CardSuitToString(suit);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Deck_ShouldHaveCorrectTotalPointValues()
        {
            // Arrange & Act
            var deck = new Deck();
            var totalPoints = deck.Cards.Sum(c => c.CardPointValue);

            // Assert
            // Expected: 4 suits × (1 Five @ 5 points + 1 Ten @ 10 points + 1 Ace @ 10 points) = 4 × 25 = 100
            Assert.Equal(100, totalPoints);
        }

        [Fact]
        public void Cards_PropertySetter_ShouldBePrivate()
        {
            // This test ensures the Cards property setter is private
            // by checking that we can't set it from outside the class
            var deck = new Deck();
            var cardsProperty = typeof(Deck).GetProperty("Cards");
            
            Assert.NotNull(cardsProperty);
            Assert.True(cardsProperty.CanRead);
            Assert.False(cardsProperty.SetMethod?.IsPublic ?? false);
        }

        [Fact]
        public void ShuffleDeck_MultipleCalls_ShouldProduceDifferentResults()
        {
            // Arrange
            var deck = new Deck();
            deck.ShuffleDeck();
            var firstShuffle = deck.Cards.ToList();

            // Act
            deck.ShuffleDeck();
            var secondShuffle = deck.Cards.ToList();

            // Assert
            // While theoretically possible to get the same order twice,
            // it's extremely unlikely with 40 cards
            Assert.NotEqual(firstShuffle, secondShuffle);
        }
    }
}