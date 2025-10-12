using Moq;
using DeuxCentsCardGame.Controllers;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;

namespace DeuxCentsCardGame.Tests.Controllers
{
    public class GameControllerTests
    {
        private readonly Mock<IPlayerManager> _mockPlayerManager;
        private readonly Mock<IDeckManager> _mockDeckManager;
        private readonly Mock<IDealingManager> _mockDealingManager;
        private readonly Mock<IBettingManager> _mockBettingManager;
        private readonly Mock<ITrumpSelectionManager> _mockTrumpSelectionManager;
        private readonly Mock<IScoringManager> _mockScoringManager;
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly Mock<IGameEventHandler> _mockEventHandler;
        private readonly GameController _gameController;

        public GameControllerTests()
        {
            _mockPlayerManager = new Mock<IPlayerManager>();
            _mockDeckManager = new Mock<IDeckManager>();
            _mockDealingManager = new Mock<IDealingManager>();
            _mockBettingManager = new Mock<IBettingManager>();
            _mockTrumpSelectionManager = new Mock<ITrumpSelectionManager>();
            _mockScoringManager = new Mock<IScoringManager>();
            _mockEventManager = new Mock<IGameEventManager>();
            _mockEventHandler = new Mock<IGameEventHandler>();

            _gameController = new GameController(
                _mockPlayerManager.Object,
                _mockDeckManager.Object,
                _mockDealingManager.Object,
                _mockBettingManager.Object,
                _mockTrumpSelectionManager.Object,
                _mockScoringManager.Object,
                _mockEventManager.Object,
                _mockEventHandler.Object
            );
        }

        [Fact]
        public void NewRound_ResetsGameState()
        {
            // Act
            _gameController.NewRound();

            // Assert
            _mockDeckManager.Verify(x => x.ResetDeck(), Times.Once);
            _mockDeckManager.Verify(x => x.ShuffleDeck(), Times.Once);
            _mockBettingManager.Verify(x => x.ResetBettingRound(), Times.Once);
        }

        [Fact]
        public void ExecuteBettingRound_CallsBettingManager()
        {
            // Act
            _gameController.ExecuteBettingRound();

            // Assert
            _mockBettingManager.Verify(x => x.ExecuteBettingRound(), Times.Once);
        }
    }
}