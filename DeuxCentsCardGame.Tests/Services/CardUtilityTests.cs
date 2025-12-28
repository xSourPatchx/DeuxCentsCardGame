using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Services;

namespace DeuxCentsCardGame.Tests.Services
{
    public class CardUtilityTests
    {
        private readonly CardUtility _cardUtility;

        public CardUtilityTests()
        {
            _cardUtility = new CardUtility();
        }

        #region StringToCardSuit Tests

        [Theory]
        [InlineData("clubs", CardSuit.Clubs)]
        [InlineData("diamonds", CardSuit.Diamonds)]
        [InlineData("hearts", CardSuit.Hearts)]
        [InlineData("spades", CardSuit.Spades)]
        public void StringToCardSuit_ConvertsLowercaseCorrectly(string input, CardSuit expected)
        {
            // Act
            var result = _cardUtility.StringToCardSuit(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("CLUBS", CardSuit.Clubs)]
        [InlineData("DIAMONDS", CardSuit.Diamonds)]
        [InlineData("HEARTS", CardSuit.Hearts)]
        [InlineData("SPADES", CardSuit.Spades)]
        public void StringToCardSuit_ConvertsUppercaseCorrectly(string input, CardSuit expected)
        {
            // Act
            var result = _cardUtility.StringToCardSuit(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Clubs")]
        [InlineData("DiAmOnDs")]
        [InlineData("HeArTs")]
        public void StringToCardSuit_ConvertsMixedCaseCorrectly(string input)
        {
            // Act & Assert - Should not throw
            var result = _cardUtility.StringToCardSuit(input);
            Assert.IsType<CardSuit>(result);
        }

        [Fact]
        public void StringToCardSuit_ThrowsExceptionForInvalidSuit()
        {
            // Arrange
            var invalidSuit = "invalid";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _cardUtility.StringToCardSuit(invalidSuit));
        }

        #endregion

        #region CardSuitToString Tests

        [Theory]
        [InlineData(CardSuit.Clubs, "clubs")]
        [InlineData(CardSuit.Diamonds, "diamonds")]
        [InlineData(CardSuit.Hearts, "hearts")]
        [InlineData(CardSuit.Spades, "spades")]
        public void CardSuitToString_ConvertsCorrectly(CardSuit suit, string expected)
        {
            // Act
            var result = _cardUtility.CardSuitToString(suit);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region GetAllCardSuits Tests

        [Fact]
        public void GetAllCardSuits_ReturnsFourSuits()
        {
            // Act
            var suits = _cardUtility.GetAllCardSuits();

            // Assert
            Assert.Equal(4, suits.Length);
        }

        [Fact]
        public void GetAllCardSuits_ContainsAllSuits()
        {
            // Act
            var suits = _cardUtility.GetAllCardSuits();

            // Assert
            Assert.Contains(CardSuit.Clubs, suits);
            Assert.Contains(CardSuit.Diamonds, suits);
            Assert.Contains(CardSuit.Hearts, suits);
            Assert.Contains(CardSuit.Spades, suits);
        }

        #endregion

        #region GetAllCardFaces Tests

        [Fact]
        public void GetAllCardFaces_ReturnsTenFaces()
        {
            // Act
            var faces = _cardUtility.GetAllCardFaces();

            // Assert
            Assert.Equal(10, faces.Length);
        }

        [Fact]
        public void GetAllCardFaces_ContainsAllFaces()
        {
            // Act
            var faces = _cardUtility.GetAllCardFaces();

            // Assert
            Assert.Contains(CardFace.Five, faces);
            Assert.Contains(CardFace.Six, faces);
            Assert.Contains(CardFace.Seven, faces);
            Assert.Contains(CardFace.Eight, faces);
            Assert.Contains(CardFace.Nine, faces);
            Assert.Contains(CardFace.Ten, faces);
            Assert.Contains(CardFace.Jack, faces);
            Assert.Contains(CardFace.Queen, faces);
            Assert.Contains(CardFace.King, faces);
            Assert.Contains(CardFace.Ace, faces);
        }

        #endregion

        #region GetCardFaceValues Tests

        [Fact]
        public void GetCardFaceValues_ReturnsTenValues()
        {
            // Act
            var faceValues = _cardUtility.GetCardFaceValues();

            // Assert
            Assert.Equal(10, faceValues.Length);
        }

        [Fact]
        public void GetCardFaceValues_ReturnsValuesFrom1To10()
        {
            // Act
            var faceValues = _cardUtility.GetCardFaceValues();

            // Assert
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, faceValues);
        }

        #endregion

        #region GetCardPointValues Tests

        [Fact]
        public void GetCardPointValues_ReturnsTenValues()
        {
            // Act
            var pointValues = _cardUtility.GetCardPointValues();

            // Assert
            Assert.Equal(10, pointValues.Length);
        }

        [Fact]
        public void GetCardPointValues_ReturnsCorrectPointDistribution()
        {
            // Act
            var pointValues = _cardUtility.GetCardPointValues();

            // Assert - Five=5, Ten=10, Ace=10, others=0
            Assert.Equal(new[] { 5, 0, 0, 0, 0, 10, 0, 0, 0, 10 }, pointValues);
        }

        [Fact]
        public void GetCardPointValues_FiveHas5Points()
        {
            // Act
            var pointValues = _cardUtility.GetCardPointValues();

            // Assert
            Assert.Equal(5, pointValues[0]); // Five is at index 0
        }

        [Fact]
        public void GetCardPointValues_TenHas10Points()
        {
            // Act
            var pointValues = _cardUtility.GetCardPointValues();

            // Assert
            Assert.Equal(10, pointValues[5]); // Ten is at index 5
        }

        [Fact]
        public void GetCardPointValues_AceHas10Points()
        {
            // Act
            var pointValues = _cardUtility.GetCardPointValues();

            // Assert
            Assert.Equal(10, pointValues[9]); // Ace is at index 9
        }

        #endregion

        #region FormatCardFace Tests

        [Theory]
        [InlineData(CardFace.Five, "5")]
        [InlineData(CardFace.Six, "6")]
        [InlineData(CardFace.Seven, "7")]
        [InlineData(CardFace.Eight, "8")]
        [InlineData(CardFace.Nine, "9")]
        [InlineData(CardFace.Ten, "10")]
        public void FormatCardFace_FormatsNumericCardsCorrectly(CardFace face, string expected)
        {
            // Act
            var result = _cardUtility.FormatCardFace(face);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(CardFace.Jack, "J")]
        [InlineData(CardFace.Queen, "Q")]
        [InlineData(CardFace.King, "K")]
        [InlineData(CardFace.Ace, "A")]
        public void FormatCardFace_FormatsFaceCardsCorrectly(CardFace face, string expected)
        {
            // Act
            var result = _cardUtility.FormatCardFace(face);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion
    }
}