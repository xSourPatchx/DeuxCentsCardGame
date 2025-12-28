using Moq;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class TrumpSelectionManagerTests
    {
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly Mock<ICardUtility> _mockCardUtility;
        private readonly TrumpSelectionManager _trumpSelectionManager;

        public TrumpSelectionManagerTests()
        {
            _mockEventManager = new Mock<IGameEventManager>();
            _mockCardUtility = new Mock<ICardUtility>();
            _trumpSelectionManager = new TrumpSelectionManager(
                _mockEventManager.Object,
                _mockCardUtility.Object);
        }

        [Fact]
        public async Task SelectTrumpSuit_ReturnsValidSuit()
        {
            // Arrange
            var winningBidder = new Player("Winner");
            _mockEventManager.Setup(x => x.RaiseTrumpSelectionInput(winningBidder))
                .ReturnsAsync("Hearts");
            _mockCardUtility.Setup(x => x.StringToCardSuit("Hearts"))
                .Returns(CardSuit.Hearts);

            // Act
            var result = await _trumpSelectionManager.SelectTrumpSuit(winningBidder);

            // Assert
            Assert.Equal(CardSuit.Hearts, result);
            _mockEventManager.Verify(x => x.RaiseTrumpSelected(CardSuit.Hearts, winningBidder), Times.Once);
        }

        [Theory]
        [InlineData("Hearts", CardSuit.Hearts)]
        [InlineData("Spades", CardSuit.Spades)]
        [InlineData("Diamonds", CardSuit.Diamonds)]
        [InlineData("Clubs", CardSuit.Clubs)]
        public async Task SelectTrumpSuit_VariousInputs_ReturnsCorrectSuit(string input, CardSuit expectedSuit)
        {
            // Arrange
            var winningBidder = new Player("Winner");
            _mockEventManager.Setup(x => x.RaiseTrumpSelectionInput(winningBidder))
                .ReturnsAsync(input);
            _mockCardUtility.Setup(x => x.StringToCardSuit(input))
                .Returns(expectedSuit);

            // Act
            var result = await _trumpSelectionManager.SelectTrumpSuit(winningBidder);

            // Assert
            Assert.Equal(expectedSuit, result);
            _mockEventManager.Verify(x => x.RaiseTrumpSelected(expectedSuit, winningBidder), Times.Once);
        }
    }
}