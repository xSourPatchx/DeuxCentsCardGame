using Moq;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Managers;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class DeckManagerTests
    {
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly Mock<IRandomService> _mockRandomService;
        private readonly Mock<ICardUtility> _mockCardUtility;
        private readonly Mock<IGameValidator> _mockGameValidator;
        private readonly DeckManager _deckManager;

        public DeckManagerTests()
        {
            _mockEventManager = new Mock<IGameEventManager>();
            _mockRandomService = new Mock<IRandomService>();
            _mockCardUtility = new Mock<ICardUtility>();
            _mockGameValidator = new Mock<IGameValidator>();
        
            _deckManager = new DeckManager(
                _mockEventManager.Object, 
                _mockRandomService.Object,
                _mockCardUtility.Object,
                _mockGameValidator.Object);
        }

        [Fact]
        public async Task ResetDeck_CreatesNewDeck()
        {
            // Arrange
            var originalDeck = _deckManager.CurrentDeck;

            // Act
            await _deckManager.ResetDeck();

            // Assert
            Assert.NotSame(originalDeck, _deckManager.CurrentDeck);
            Assert.Equal(40, _deckManager.CurrentDeck.Cards.Count);
        }

        [Fact]
        public async Task ShuffleDeck_ChangesCardOrder()
        {
            // Arrange
            _mockRandomService.Setup(x => x.Next(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((min, max) => max - 1); // Always return last index

            var originalOrder = _deckManager.CurrentDeck.Cards.ToList();

            // Act
            await _deckManager.ShuffleDeck();

            // Assert
            Assert.Equal(40, _deckManager.CurrentDeck.Cards.Count);
            _mockEventManager.Verify(x => x.RaiseDeckShuffled(It.IsAny<string>()), Times.Once);
        }
        
        [Fact]
        public async Task CutDeck_WithValidPosition_CutsDeck()
        {
            // Arrange
            int cutPosition = 20;
            var originalFirstCard = _deckManager.CurrentDeck.Cards[0];
            var originalCardAtCutPosition = _deckManager.CurrentDeck.Cards[cutPosition];

            // Act
            await _deckManager.CutDeck(cutPosition);

            // Assert
            Assert.Equal(originalCardAtCutPosition, _deckManager.CurrentDeck.Cards[0]);
            Assert.Equal(40, _deckManager.CurrentDeck.Cards.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(40)]
        [InlineData(50)]
        public async Task CutDeck_WithInvalidPosition_ThrowsException(int cutPosition)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
                _deckManager.CutDeck(cutPosition));
        }
    }
}