using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Services;
using Moq;

namespace DeuxCentsCardGame.Tests.Models
{
    public class DeckTests
    {
        private readonly Mock<GameValidator> _mockValidator;
        private readonly CardUtility _cardUtility;

        public DeckTests()
        {
            _cardUtility = new CardUtility();
            _mockValidator = new Mock<GameValidator>(
                Mock.Of<Interfaces.GameConfig.IGameConfig>(),
                Mock.Of<Interfaces.Events.IGameEventManager>(),
                _cardUtility,
                new List<Player>()
            );
            _mockValidator.Setup(v => v.ValidateCard(
                It.IsAny<CardSuit>(),
                It.IsAny<CardFace>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            ));
        }

        [Fact]
        public void Deck_Constructor_Initializes40Cards()
        {
            // Act
            var deck = new Deck(_cardUtility, _mockValidator.Object);

            // Assert
            Assert.Equal(40, deck.Cards.Count);
        }

        [Fact]
        public void Deck_ContainsAllFourSuits()
        {
            // Act
            var deck = new Deck(_cardUtility, _mockValidator.Object);

            // Assert
            var suits = deck.Cards.Select(c => c.CardSuit).Distinct().ToList();
            Assert.Equal(4, suits.Count);
            Assert.Contains(CardSuit.Clubs, suits);
            Assert.Contains(CardSuit.Diamonds, suits);
            Assert.Contains(CardSuit.Hearts, suits);
            Assert.Contains(CardSuit.Spades, suits);
        }

        [Fact]
        public void Deck_ContainsTenCardsPerSuit()
        {
            // Act
            var deck = new Deck(_cardUtility, _mockValidator.Object);

            // Assert
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                var cardsInSuit = deck.Cards.Count(c => c.CardSuit == suit);
                Assert.Equal(10, cardsInSuit);
            }
        }

        [Fact]
        public void Deck_ContainsAllCardFaces()
        {
            // Act
            var deck = new Deck(_cardUtility, _mockValidator.Object);

            // Assert
            var faces = deck.Cards.Select(c => c.CardFace).Distinct().ToList();
            Assert.Equal(10, faces.Count);
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

        [Fact]
        public void Deck_CardsHaveCorrectFaceValues()
        {
            // Act
            var deck = new Deck(_cardUtility, _mockValidator.Object);

            // Assert
            var faceValues = deck.Cards.Select(c => c.CardFaceValue).Distinct().OrderBy(v => v).ToList();
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, faceValues);
        }

        [Fact]
        public void Deck_FiveCardsHave5Points()
        {
            // Act
            var deck = new Deck(_cardUtility, _mockValidator.Object);

            // Assert
            var fiveCards = deck.Cards.Where(c => c.CardFace == CardFace.Five).ToList();
            Assert.Equal(4, fiveCards.Count); // One per suit
            Assert.All(fiveCards, card => Assert.Equal(5, card.CardPointValue));
        }

        [Fact]
        public void Deck_TenAndAceCardsHave10Points()
        {
            // Act
            var deck = new Deck(_cardUtility, _mockValidator.Object);

            // Assert
            var tenCards = deck.Cards.Where(c => c.CardFace == CardFace.Ten).ToList();
            var aceCards = deck.Cards.Where(c => c.CardFace == CardFace.Ace).ToList();

            Assert.Equal(4, tenCards.Count);
            Assert.Equal(4, aceCards.Count);
            Assert.All(tenCards, card => Assert.Equal(10, card.CardPointValue));
            Assert.All(aceCards, card => Assert.Equal(10, card.CardPointValue));
        }

        [Fact]
        public void Deck_OtherCardsHave0Points()
        {
            // Act
            var deck = new Deck(_cardUtility, _mockValidator.Object);

            // Assert
            var zeroPointFaces = new[] { CardFace.Six, CardFace.Seven, CardFace.Eight, 
                                        CardFace.Nine, CardFace.Jack, CardFace.Queen, CardFace.King };
            var zeroPointCards = deck.Cards.Where(c => zeroPointFaces.Contains(c.CardFace)).ToList();

            Assert.Equal(28, zeroPointCards.Count); // 7 faces * 4 suits
            Assert.All(zeroPointCards, card => Assert.Equal(0, card.CardPointValue));
        }

        [Fact]
        public void Deck_TotalPointsEqual120()
        {
            // Act
            var deck = new Deck(_cardUtility, _mockValidator.Object);

            // Assert
            var totalPoints = deck.Cards.Sum(c => c.CardPointValue);
            Assert.Equal(120, totalPoints);
        }

        [Fact]
        public void Deck_CallsValidateCardForEachCard()
        {
            // Act
            var deck = new Deck(_cardUtility, _mockValidator.Object);

            // Assert
            _mockValidator.Verify(v => v.ValidateCard(
                It.IsAny<CardSuit>(),
                It.IsAny<CardFace>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            ), Times.Exactly(40));
        }
    }
}