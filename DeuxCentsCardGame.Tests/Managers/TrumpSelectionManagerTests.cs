using Moq;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Managers
{
    public class TrumpSelectionManagerTests
    {
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly TrumpSelectionManager _trumpSelectionManager;

        public TrumpSelectionManagerTests()
        {
            _mockEventManager = new Mock<IGameEventManager>();
            _trumpSelectionManager = new TrumpSelectionManager(_mockEventManager.Object);
        }

        [Fact]
        public void SelectTrumpSuit_ReturnsValidSuit()
        {
            // Arrange
            var winningBidder = new Player("Winner");
            _mockEventManager.Setup(x => x.RaiseTrumpSelectionInput(winningBidder))
                .Returns("Hearts");

            // Act
            var result = _trumpSelectionManager.SelectTrumpSuit(winningBidder);

            // Assert
            Assert.Equal(CardSuit.Hearts, result);
            _mockEventManager.Verify(x => x.RaiseTrumpSelected(CardSuit.Hearts, winningBidder), Times.Once);
        }

        [Theory]
        [InlineData("Hearts", CardSuit.Hearts)]
        [InlineData("Spades", CardSuit.Spades)]
        [InlineData("Diamonds", CardSuit.Diamonds)]
        [InlineData("Clubs", CardSuit.Clubs)]
        public void SelectTrumpSuit_VariousInputs_ReturnsCorrectSuit(string input, CardSuit expectedSuit)
        {
            // Arrange
            var winningBidder = new Player("Winner");
            _mockEventManager.Setup(x => x.RaiseTrumpSelectionInput(winningBidder))
                .Returns(input);

            // Act
            var result = _trumpSelectionManager.SelectTrumpSuit(winningBidder);

            // Assert
            Assert.Equal(expectedSuit, result);
        }
    }
}