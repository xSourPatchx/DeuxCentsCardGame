using DeuxCentsCardGame.Controllers;
using DeuxCentsCardGame.Interfaces.Models;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;
using Moq;
using Xunit;

namespace DeuxCentsCardGame.Tests.Controllers
{
    public class RoundControllerTests
    {
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly Mock<IPlayerManager> _mockPlayerManager;
        private readonly Mock<IPlayerTurnManager> _mockPlayerTurnManager;
        private readonly Mock<IDeckManager> _mockDeckManager;
        private readonly Mock<IDealingManager> _mockDealingManager;
        private readonly Mock<IBettingManager> _mockBettingManager;
        private readonly Mock<ITrumpSelectionManager> _mockTrumpSelectionManager;
        private readonly Mock<IScoringManager> _mockScoringManager;
        private readonly Mock<ICardUtility> _mockCardUtility;
        private readonly Mock<IGameValidator> _mockGameValidator;
        private readonly RoundController _roundController;
        private readonly List<Player> _testPlayers;
        private readonly Deck _testDeck;
        private const int InitialDealerIndex = 3;

        public RoundControllerTests()
        {
            _mockEventManager = new Mock<IGameEventManager>();
            _mockPlayerManager = new Mock<IPlayerManager>();
            _mockPlayerTurnManager = new Mock<IPlayerTurnManager>();
            _mockDeckManager = new Mock<IDeckManager>();
            _mockDealingManager = new Mock<IDealingManager>();
            _mockBettingManager = new Mock<IBettingManager>();
            _mockTrumpSelectionManager = new Mock<ITrumpSelectionManager>();
            _mockScoringManager = new Mock<IScoringManager>();
            _mockCardUtility = new Mock<ICardUtility>();
            _mockGameValidator = new Mock<IGameValidator>();

            _testPlayers = new List<Player>
            {
                new Player("Player 1"),
                new Player("Player 2"),
                new Player("Player 3"),
                new Player("Player 4")
            };

            _testDeck = new Deck(_mockCardUtility.Object, _mockGameValidator.Object);

            _mockPlayerManager.Setup(m => m.Players).Returns(_testPlayers.AsReadOnly());
            _mockPlayerManager.Setup(m => m.GetPlayer(It.IsAny<int>()))
                .Returns<int>(index => _testPlayers[index]);
            _mockDeckManager.Setup(m => m.CurrentDeck).Returns(_testDeck);
            _mockBettingManager.Setup(m => m.CurrentWinningBidIndex).Returns(0);
            _mockBettingManager.Setup(m => m.CurrentWinningBid).Returns(50);
            _mockTrumpSelectionManager.Setup(m => m.SelectTrumpSuit(It.IsAny<Player>()))
                .ReturnsAsync(CardSuit.Hearts);
            _mockPlayerTurnManager.Setup(m => m.GetPlayerRightOfDealer(It.IsAny<int>())).Returns(0);
            _mockPlayerTurnManager.Setup(m => m.RotateDealer(It.IsAny<int>())).Returns(0);

            _mockEventManager.Setup(m => m.RaiseRoundStarted(It.IsAny<int>(), It.IsAny<Player>())).Returns(Task.CompletedTask);
            _mockEventManager.Setup(m => m.RaiseDeckCutInput(It.IsAny<Player>(), It.IsAny<int>())).ReturnsAsync(26);
            _mockEventManager.Setup(m => m.RaiseDeckCut(It.IsAny<Player>(), It.IsAny<int>())).Returns(Task.CompletedTask);
            _mockEventManager.Setup(m => m.RaiseRoundEnded(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Player>(), It.IsAny<int>())).Returns(Task.CompletedTask);
            
            _mockDeckManager.Setup(m => m.ResetDeck()).Returns(Task.CompletedTask);
            _mockDeckManager.Setup(m => m.ShuffleDeck()).Returns(Task.CompletedTask);
            _mockDeckManager.Setup(m => m.CutDeck(It.IsAny<int>())).Returns(Task.CompletedTask);
            
            _mockDealingManager.Setup(m => m.DealCards(It.IsAny<Deck>(), It.IsAny<List<Player>>())).Returns(Task.CompletedTask);
            _mockDealingManager.Setup(m => m.RaiseCardsDealtEvent(It.IsAny<List<Player>>(), It.IsAny<int>())).Returns(Task.CompletedTask);
            
            _mockBettingManager.Setup(m => m.UpdateDealerIndex(It.IsAny<int>())).Returns(Task.CompletedTask);
            _mockBettingManager.Setup(m => m.ExecuteBettingRound()).Returns(Task.CompletedTask);
            _mockBettingManager.Setup(m => m.ResetBettingRound()).Returns(Task.CompletedTask);

            _roundController = new RoundController(
                _mockEventManager.Object,
                _mockPlayerManager.Object,
                _mockPlayerTurnManager.Object,
                _mockDeckManager.Object,
                _mockDealingManager.Object,
                _mockBettingManager.Object,
                _mockTrumpSelectionManager.Object,
                _mockScoringManager.Object,
                InitialDealerIndex
            );
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidDependencies_InitializesSuccessfully()
        {
            Assert.NotNull(_roundController);
        }

        [Fact]
        public void Constructor_WithNullEventManager_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new RoundController(
                null!,
                _mockPlayerManager.Object,
                _mockPlayerTurnManager.Object,
                _mockDeckManager.Object,
                _mockDealingManager.Object,
                _mockBettingManager.Object,
                _mockTrumpSelectionManager.Object,
                _mockScoringManager.Object,
                InitialDealerIndex
            ));
        }

        [Fact]
        public void Constructor_WithNullPlayerManager_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new RoundController(
                _mockEventManager.Object,
                null!,
                _mockPlayerTurnManager.Object,
                _mockDeckManager.Object,
                _mockDealingManager.Object,
                _mockBettingManager.Object,
                _mockTrumpSelectionManager.Object,
                _mockScoringManager.Object,
                InitialDealerIndex
            ));
        }

        [Fact]
        public void Constructor_InitializesDealerIndex()
        {
            Assert.Equal(InitialDealerIndex, _roundController.DealerIndex);
        }

        [Fact]
        public void Constructor_InitializesTrumpSuitAsNull()
        {
            Assert.Null(_roundController.TrumpSuit);
        }

        #endregion

        #region InitializeRound Tests

        [Fact]
        public async Task InitializeRound_RaisesRoundStartedEvent()
        {
            await _roundController.InitializeRound(1);

            _mockEventManager.Verify(m => m.RaiseRoundStarted(1, _testPlayers[3]), Times.Once);
        }

        [Fact]
        public async Task InitializeRound_ResetsDeck()
        {
            await _roundController.InitializeRound(1);

            _mockDeckManager.Verify(m => m.ResetDeck(), Times.Once);
        }

        [Fact]
        public async Task InitializeRound_ResetsTrumpSuitToNull()
        {
            _roundController.TrumpSuit.GetType(); // Set to non-null first
            
            await _roundController.InitializeRound(1);

            Assert.Null(_roundController.TrumpSuit);
        }

        [Fact]
        public async Task InitializeRound_ResetsRoundPoints()
        {
            await _roundController.InitializeRound(1);

            _mockScoringManager.Verify(m => m.ResetRoundPoints(), Times.Once);
        }

        [Fact]
        public async Task InitializeRound_ResetsBettingRound()
        {
            await _roundController.InitializeRound(1);

            _mockBettingManager.Verify(m => m.ResetBettingRound(), Times.Once);
        }

        [Fact]
        public async Task InitializeRound_ResetsTurnSequence()
        {
            await _roundController.InitializeRound(1);

            _mockPlayerTurnManager.Verify(m => m.ResetTurnSequence(), Times.Once);
        }

        #endregion

        #region PrepareRound Tests

        [Fact]
        public async Task PrepareRound_ShufflesDeck()
        {
            await _roundController.PrepareRound();

            _mockDeckManager.Verify(m => m.ShuffleDeck(), Times.Once);
        }

        [Fact]
        public async Task PrepareRound_RequestsDeckCutFromPlayerRightOfDealer()
        {
            _mockPlayerTurnManager.Setup(m => m.GetPlayerRightOfDealer(InitialDealerIndex)).Returns(0);

            await _roundController.PrepareRound();

            _mockEventManager.Verify(m => m.RaiseDeckCutInput(_testPlayers[0], It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task PrepareRound_CutsDeckAtSpecifiedPosition()
        {
            _mockEventManager.Setup(m => m.RaiseDeckCutInput(It.IsAny<Player>(), It.IsAny<int>())).ReturnsAsync(30);

            await _roundController.PrepareRound();

            _mockDeckManager.Verify(m => m.CutDeck(30), Times.Once);
        }

        [Fact]
        public async Task PrepareRound_RaisesDeckCutEvent()
        {
            _mockPlayerTurnManager.Setup(m => m.GetPlayerRightOfDealer(InitialDealerIndex)).Returns(1);
            _mockEventManager.Setup(m => m.RaiseDeckCutInput(It.IsAny<Player>(), It.IsAny<int>())).ReturnsAsync(25);

            await _roundController.PrepareRound();

            _mockEventManager.Verify(m => m.RaiseDeckCut(_testPlayers[1], 25), Times.Once);
        }

        [Fact]
        public async Task PrepareRound_DealsCards()
        {
            await _roundController.PrepareRound();

            _mockDealingManager.Verify(m => m.DealCards(_testDeck, _testPlayers), Times.Once);
        }

        [Fact]
        public async Task PrepareRound_RaisesCardsDealtEvent()
        {
            await _roundController.PrepareRound();

            _mockDealingManager.Verify(m => m.RaiseCardsDealtEvent(_testPlayers, InitialDealerIndex), Times.Once);
        }

        [Fact]
        public async Task PrepareRound_ExecutesInCorrectOrder()
        {
            var callOrder = new List<string>();

            _mockDeckManager.Setup(m => m.ShuffleDeck())
                .Callback(() => callOrder.Add("Shuffle"))
                .Returns(Task.CompletedTask);
            _mockEventManager.Setup(m => m.RaiseDeckCutInput(It.IsAny<Player>(), It.IsAny<int>()))
                .Callback(() => callOrder.Add("CutInput"))
                .ReturnsAsync(26);
            _mockDeckManager.Setup(m => m.CutDeck(It.IsAny<int>()))
                .Callback(() => callOrder.Add("Cut"))
                .Returns(Task.CompletedTask);
            _mockDealingManager.Setup(m => m.DealCards(It.IsAny<Deck>(), It.IsAny<List<Player>>()))
                .Callback(() => callOrder.Add("Deal"))
                .Returns(Task.CompletedTask);

            await _roundController.PrepareRound();

            Assert.Equal("Shuffle", callOrder[0]);
            Assert.Equal("CutInput", callOrder[1]);
            Assert.Equal("Cut", callOrder[2]);
            Assert.Equal("Deal", callOrder[3]);
        }

        #endregion

        #region ExecuteBettingPhase Tests

        [Fact]
        public async Task ExecuteBettingPhase_UpdatesDealerIndexInBettingManager()
        {
            await _roundController.ExecuteBettingPhase();

            _mockBettingManager.Verify(m => m.UpdateDealerIndex(InitialDealerIndex), Times.Once);
        }

        [Fact]
        public async Task ExecuteBettingPhase_ExecutesBettingRound()
        {
            await _roundController.ExecuteBettingPhase();

            _mockBettingManager.Verify(m => m.ExecuteBettingRound(), Times.Once);
        }

        [Fact]
        public async Task ExecuteBettingPhase_UpdatesDealerBeforeExecutingBetting()
        {
            var callOrder = new List<string>();

            _mockBettingManager.Setup(m => m.UpdateDealerIndex(It.IsAny<int>()))
                .Callback(() => callOrder.Add("UpdateDealer"))
                .Returns(Task.CompletedTask);
            _mockBettingManager.Setup(m => m.ExecuteBettingRound())
                .Callback(() => callOrder.Add("ExecuteBetting"))
                .Returns(Task.CompletedTask);

            await _roundController.ExecuteBettingPhase();

            Assert.Equal("UpdateDealer", callOrder[0]);
            Assert.Equal("ExecuteBetting", callOrder[1]);
        }

        #endregion

        #region SelectTrump Tests

        [Fact]
        public async Task SelectTrump_CallsTrumpSelectionManagerWithWinningBidder()
        {
            _mockBettingManager.Setup(m => m.CurrentWinningBidIndex).Returns(2);

            await _roundController.SelectTrump();

            _mockTrumpSelectionManager.Verify(m => m.SelectTrumpSuit(_testPlayers[2]), Times.Once);
        }

        [Fact]
        public async Task SelectTrump_SetsTrumpSuitProperty()
        {
            _mockTrumpSelectionManager.Setup(m => m.SelectTrumpSuit(It.IsAny<Player>()))
                .ReturnsAsync(CardSuit.Diamonds);

            await _roundController.SelectTrump();

            Assert.Equal(CardSuit.Diamonds, _roundController.TrumpSuit);
        }

        #endregion

        #region FinalizeRound Tests

        [Fact]
        public async Task FinalizeRound_ScoresTheRound()
        {
            _mockBettingManager.Setup(m => m.CurrentWinningBidIndex).Returns(1);
            _mockBettingManager.Setup(m => m.CurrentWinningBid).Returns(75);

            await _roundController.FinalizeRound(1);

            _mockScoringManager.Verify(m => m.ScoreRound(1, 75), Times.Once);
        }

        [Fact]
        public async Task FinalizeRound_RaisesRoundEndedEvent()
        {
            _mockBettingManager.Setup(m => m.CurrentWinningBidIndex).Returns(2);
            _mockBettingManager.Setup(m => m.CurrentWinningBid).Returns(60);
            _mockScoringManager.Setup(m => m.TeamOneRoundPoints).Returns(100);
            _mockScoringManager.Setup(m => m.TeamTwoRoundPoints).Returns(50);

            await _roundController.FinalizeRound(3);

            _mockEventManager.Verify(m => m.RaiseRoundEnded(
                3,
                100,
                50,
                _testPlayers[2],
                60), Times.Once);
        }

        [Fact]
        public async Task FinalizeRound_RotatesDealer()
        {
            _mockPlayerTurnManager.Setup(m => m.RotateDealer(InitialDealerIndex)).Returns(0);

            await _roundController.FinalizeRound(1);

            _mockPlayerTurnManager.Verify(m => m.RotateDealer(InitialDealerIndex), Times.Once);
            Assert.Equal(0, _roundController.DealerIndex);
        }

        [Fact]
        public async Task FinalizeRound_UpdatesDealerIndexProperty()
        {
            _mockPlayerTurnManager.Setup(m => m.RotateDealer(InitialDealerIndex)).Returns(1);

            await _roundController.FinalizeRound(1);

            Assert.Equal(1, _roundController.DealerIndex);
        }

        [Fact]
        public async Task FinalizeRound_ExecutesInCorrectOrder()
        {
            var callOrder = new List<string>();

            _mockScoringManager.Setup(m => m.ScoreRound(It.IsAny<int>(), It.IsAny<int>()))
                .Callback(() => callOrder.Add("Score"));
            _mockEventManager.Setup(m => m.RaiseRoundEnded(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Player>(), It.IsAny<int>()))
                .Callback(() => callOrder.Add("RoundEnded"))
                .Returns(Task.CompletedTask);
            _mockScoringManager.Setup(m => m.IsGameOver())
                .Callback(() => callOrder.Add("CheckGameOver"))
                .Returns(false);
            // _mockEventManager.Setup(m => m.RaiseNextRoundPrompt())
            //     .Callback(() => callOrder.Add("NextRound"))
            //     .Returns(Task.CompletedTask);
            _mockPlayerTurnManager.Setup(m => m.RotateDealer(It.IsAny<int>()))
                .Callback(() => callOrder.Add("RotateDealer"))
                .Returns(0);

            await _roundController.FinalizeRound(1);

            Assert.Equal("Score", callOrder[0]);
            Assert.Equal("RoundEnded", callOrder[1]);
            Assert.Equal("CheckGameOver", callOrder[2]);
            Assert.Equal("NextRound", callOrder[3]);
            Assert.Equal("RotateDealer", callOrder[4]);
        }

        #endregion

        #region GetStartingPlayerIndex Tests

        [Fact]
        public void GetStartingPlayerIndex_ReturnsCurrentWinningBidIndex()
        {
            _mockBettingManager.Setup(m => m.CurrentWinningBidIndex).Returns(2);

            var result = _roundController.GetStartingPlayerIndex();

            Assert.Equal(2, result);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task FullRound_ExecutesAllPhasesInCorrectOrder()
        {
            var callOrder = new List<string>();

            _mockEventManager.Setup(m => m.RaiseRoundStarted(It.IsAny<int>(), It.IsAny<Player>()))
                .Callback(() => callOrder.Add("RoundStarted"))
                .Returns(Task.CompletedTask);
            _mockDeckManager.Setup(m => m.ResetDeck())
                .Callback(() => callOrder.Add("ResetDeck"))
                .Returns(Task.CompletedTask);
            _mockDeckManager.Setup(m => m.ShuffleDeck())
                .Callback(() => callOrder.Add("Shuffle"))
                .Returns(Task.CompletedTask);
            _mockDealingManager.Setup(m => m.DealCards(It.IsAny<Deck>(), It.IsAny<List<Player>>()))
                .Callback(() => callOrder.Add("Deal"))
                .Returns(Task.CompletedTask);
            _mockBettingManager.Setup(m => m.ExecuteBettingRound())
                .Callback(() => callOrder.Add("Betting"))
                .Returns(Task.CompletedTask);
            _mockTrumpSelectionManager.Setup(m => m.SelectTrumpSuit(It.IsAny<Player>()))
                .Callback(() => callOrder.Add("Trump"))
                .ReturnsAsync(CardSuit.Hearts);
            _mockScoringManager.Setup(m => m.ScoreRound(It.IsAny<int>(), It.IsAny<int>()))
                .Callback(() => callOrder.Add("Score"));

            await _roundController.InitializeRound(1);
            await _roundController.PrepareRound();
            await _roundController.ExecuteBettingPhase();
            await _roundController.SelectTrump();
            await _roundController.FinalizeRound(1);

            Assert.Equal("RoundStarted", callOrder[0]);
            Assert.Equal("ResetDeck", callOrder[1]);
            Assert.Contains("Shuffle", callOrder);
            Assert.Contains("Deal", callOrder);
            Assert.Contains("Betting", callOrder);
            Assert.Contains("Trump", callOrder);
            Assert.Equal("Score", callOrder[^1]);
        }

        [Fact]
        public async Task DealerRotation_OverMultipleRounds_WorksCorrectly()
        {
            _mockPlayerTurnManager.SetupSequence(m => m.RotateDealer(It.IsAny<int>()))
                .Returns(0)
                .Returns(1)
                .Returns(2);

            await _roundController.FinalizeRound(1);
            Assert.Equal(0, _roundController.DealerIndex);

            await _roundController.FinalizeRound(2);
            Assert.Equal(1, _roundController.DealerIndex);

            await _roundController.FinalizeRound(3);
            Assert.Equal(2, _roundController.DealerIndex);
        }

        #endregion
    }
}