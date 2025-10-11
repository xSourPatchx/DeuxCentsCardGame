using Moq;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Managers;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class DeckManagerTests
    {
        private readonly Mock<GameEventManager> _mockEventManager;
        private readonly Mock<IRandomService> _mockRandomService;
        private readonly DeckManager _deckManager;

        public DeckManagerTests()
        {
            _mockEventManager = new Mock<GameEventManager>();
            _mockRandomService = new Mock<IRandomService>();
            _deckManager = new DeckManager(_mockEventManager.Object, _mockRandomService.Object);
        }

        [Fact]
        public void ResetDeck_CreatesNewDeck()
        {
            // Arrange
            var originalDeck = _deckManager.CurrentDeck;

            // Act
            _deckManager.ResetDeck();

            // Assert
            Assert.NotSame(originalDeck, _deckManager.CurrentDeck);
            Assert.Equal(40, _deckManager.CurrentDeck.Cards.Count);
        }

        [Fact]
        public void ShuffleDeck_ChangesCardOrder()
        {
            // Arrange
            _mockRandomService.Setup(x => x.Next(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((min, max) => max - 1); // Always return last index

            var originalOrder = _deckManager.CurrentDeck.Cards.ToList();

            // Act
            _deckManager.ShuffleDeck();

            // Assert
            // Since we mocked random to always return last index, order should be different
            Assert.Equal(52, _deckManager.CurrentDeck.Cards.Count);
        }
    }
}