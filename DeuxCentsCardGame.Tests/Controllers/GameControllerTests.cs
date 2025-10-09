using Moq;
using DeuxCentsCardGame.Controllers;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Tests.Controllers
{
    public class GameControllerTests
    {
        private readonly Mock<PlayerManager> _mockPlayerManager;
        private readonly Mock<DeckManager> _mockDeckManager;
        private readonly Mock<DealingManager> _mockDealingManager;
        private readonly Mock<BettingManager> _mockBettingManager;
        private readonly Mock<TrumpSelectionManager> _mockTrumpSelectionManager;
        private readonly Mock<ScoringManager> _mockScoringManager;
        private readonly Mock<GameEventManager> _mockEventManager;
        private readonly Mock<GameEventHandler> _mockEventHandler;
        private readonly GameController _gameController;

        public GameControllerTests()
        {
            _mockPlayerManager = new Mock<PlayerManager>(Mock.Of<GameEventManager>());
            _mockDeckManager = new Mock<DeckManager>(Mock.Of<GameEventManager>(), Mock.Of<IRandomService>());
            _mockDealingManager = new Mock<DealingManager>(Mock.Of<GameEventManager>());
            _mockBettingManager = new Mock<BettingManager>(new List<Player>(), 0, Mock.Of<GameEventManager>(), Mock.Of<IGameConfig>());
            _mockTrumpSelectionManager = new Mock<TrumpSelectionManager>(Mock.Of<GameEventManager>());
            _mockScoringManager = new Mock<ScoringManager>(new List<Player>(), Mock.Of<GameEventManager>());
            _mockEventManager = new Mock<GameEventManager>();
            _mockEventHandler = new Mock<GameEventHandler>();

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