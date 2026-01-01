using DeuxCentsCardGame.Controllers;
using DeuxCentsCardGame.GameStates;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;
using Moq;

namespace DeuxCentsCardGame.Tests.Controllers
{
    public class GameControllerTests
    {
        private readonly Mock<RoundController> _mockRoundController;
        private readonly Mock<TrickController> _mockTrickController;
        private readonly Mock<IScoringManager> _mockScoringManager;
        private readonly Mock<IGameEventManager> _mockEventManager;
        private readonly Mock<IGameEventHandler> _mockEventHandler;
        private readonly GameController _gameController;

        public GameControllerTests()
        {
            _mockRoundController = new Mock<RoundController>();
            _mockTrickController = new Mock<TrickController>();
            _mockScoringManager = new Mock<IScoringManager>();
            _mockEventManager = new Mock<IGameEventManager>();
            _mockEventHandler = new Mock<IGameEventHandler>();

            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(false);
            _mockRoundController.Setup(m => m.GetStartingPlayerIndex()).Returns(0);
            _mockRoundController.Setup(m => m.TrumpSuit).Returns(CardSuit.Hearts);

            _mockRoundController.Setup(m => m.InitializeRound(It.IsAny<int>())).Returns(Task.CompletedTask);
            _mockRoundController.Setup(m => m.PrepareRound()).Returns(Task.CompletedTask);
            _mockRoundController.Setup(m => m.ExecuteBettingPhase()).Returns(Task.CompletedTask);
            _mockRoundController.Setup(m => m.SelectTrump()).Returns(Task.CompletedTask);
            _mockRoundController.Setup(m => m.FinalizeRound(It.IsAny<int>())).Returns(Task.CompletedTask);
            
            _mockTrickController.Setup(m => m.PlayAllTricks(It.IsAny<int>(), It.IsAny<CardSuit?>())).Returns(Task.CompletedTask);
            
            _mockEventManager.Setup(m => m.RaiseStateChanged(It.IsAny<GameState>(), It.IsAny<GameState>())).Returns(Task.CompletedTask);
            _mockEventManager.Setup(m => m.RaiseGamePaused(It.IsAny<GameState>())).Returns(Task.CompletedTask);
            _mockEventManager.Setup(m => m.RaiseGameResumed(It.IsAny<GameState>())).Returns(Task.CompletedTask);

            _gameController = new GameController(
                _mockRoundController.Object,
                _mockTrickController.Object,
                _mockScoringManager.Object,
                _mockEventManager.Object,
                _mockEventHandler.Object
            );
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidDependencies_InitializesSuccessfully()
        {
            Assert.NotNull(_gameController);
        }

        [Fact]
        public void Constructor_WithNullRoundController_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GameController(
                null!,
                _mockTrickController.Object,
                _mockScoringManager.Object,
                _mockEventManager.Object,
                _mockEventHandler.Object
            ));
        }

        [Fact]
        public void Constructor_WithNullTrickController_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GameController(
                _mockRoundController.Object,
                null!,
                _mockScoringManager.Object,
                _mockEventManager.Object,
                _mockEventHandler.Object
            ));
        }

        [Fact]
        public void Constructor_WithNullScoringManager_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GameController(
                _mockRoundController.Object,
                _mockTrickController.Object,
                null!,
                _mockEventManager.Object,
                _mockEventHandler.Object
            ));
        }

        [Fact]
        public void Constructor_WithNullEventManager_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GameController(
                _mockRoundController.Object,
                _mockTrickController.Object,
                _mockScoringManager.Object,
                null!,
                _mockEventHandler.Object
            ));
        }

        [Fact]
        public void Constructor_WithNullEventHandler_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GameController(
                _mockRoundController.Object,
                _mockTrickController.Object,
                _mockScoringManager.Object,
                _mockEventManager.Object,
                null!
            ));
        }

        #endregion

        #region State Management Tests

        [Fact]
        public void GetCurrentState_InitiallyReturnsInitialization()
        {
            Assert.Equal(GameState.Initialization, _gameController.GetCurrentState());
        }

        [Fact]
        public void GetCurrentRound_InitiallyReturnsZero()
        {
            Assert.Equal(0, _gameController.GetCurrentRound());
        }

        [Fact]
        public void GetCurrentTrick_InitiallyReturnsZero()
        {
            Assert.Equal(0, _gameController.GetCurrentTrick());
        }

        [Fact]
        public async Task TransitionToState_UpdatesCurrentState()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.Betting);

            Assert.Equal(GameState.Betting, _gameController.GetCurrentState());
        }

        [Fact]
        public async Task TransitionToState_RaisesStateChangedEvent()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.Betting);

            _mockEventManager.Verify(m => m.RaiseStateChanged(
                GameState.Initialization, 
                GameState.Betting), Times.Once);
        }

        [Fact]
        public async Task PauseGame_SetsPausedStateToTrue()
        {
            await _gameController.PauseGame();

            Assert.True(_gameController.IsPaused());
        }

        [Fact]
        public async Task PauseGame_RaisesGamePausedEvent()
        {
            await _gameController.PauseGame();

            _mockEventManager.Verify(m => m.RaiseGamePaused(GameState.Initialization), Times.Once);
        }

        [Fact]
        public async Task PauseGame_WhenAlreadyPaused_DoesNotRaiseEventAgain()
        {
            await _gameController.PauseGame();
            await _gameController.PauseGame();

            _mockEventManager.Verify(m => m.RaiseGamePaused(It.IsAny<GameState>()), Times.Once);
        }

        [Fact]
        public async Task ResumeGame_ClearsPausedState()
        {
            await _gameController.PauseGame();
            await _gameController.ResumeGame();

            Assert.False(_gameController.IsPaused());
        }

        [Fact]
        public async Task ResumeGame_RaisesGameResumedEvent()
        {
            await _gameController.PauseGame();
            await _gameController.ResumeGame();

            _mockEventManager.Verify(m => m.RaiseGameResumed(GameState.Initialization), Times.Once);
        }

        [Fact]
        public async Task TransitionToState_DoesNotTransition_WhenPaused()
        {
            await _gameController.PauseGame();
            var stateBeforePause = _gameController.GetCurrentState();

            await _gameController.TransitionToState(GameState.Betting);

            Assert.Equal(stateBeforePause, _gameController.GetCurrentState());
        }

        [Fact]
        public async Task IsPaused_InitiallyReturnsFalse()
        {
            Assert.False(_gameController.IsPaused());
        }

        #endregion

        #region Game Flow Tests

        [Fact]
        public async Task StartGame_UnsubscribesFromEvents_WhenGameEnds()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.StartGame();

            _mockEventHandler.Verify(m => m.UnsubscribeFromEvents(), Times.Once);
        }

        [Fact]
        public async Task NewRound_TransitionsToRoundStartState()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.NewRound();
            await Task.Delay(50);

            _mockEventManager.Verify(m => m.RaiseStateChanged(
                It.IsAny<GameState>(), 
                GameState.RoundStart), Times.Once);
        }

        #endregion

        #region State Handler Tests - Initialization

        [Fact]
        public async Task HandleInitialization_SetsCurrentRoundToOne()
        {
            await _gameController.TransitionToState(GameState.Initialization);
            await Task.Delay(50);

            Assert.Equal(1, _gameController.GetCurrentRound());
        }

        [Fact]
        public async Task HandleInitialization_AutomaticallyTransitionsToRoundStart()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.Initialization);
            await Task.Delay(50);

            _mockEventManager.Verify(m => m.RaiseStateChanged(
                GameState.Initialization,
                GameState.RoundStart), Times.Once);
        }

        #endregion

        #region State Handler Tests - RoundStart

        [Fact]
        public async Task HandleRoundStart_CallsInitializeRound()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.RoundStart);

            _mockRoundController.Verify(m => m.InitializeRound(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task HandleRoundStart_TransitionsToDeckPreparation()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.RoundStart);
            await Task.Delay(50);

            _mockEventManager.Verify(m => m.RaiseStateChanged(
                GameState.RoundStart,
                GameState.DeckPreparation), Times.Once);
        }

        #endregion

        #region State Handler Tests - DeckPreparation

        [Fact]
        public async Task HandleDeckPreparation_CallsPrepareRound()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.DeckPreparation);

            _mockRoundController.Verify(m => m.PrepareRound(), Times.Once);
        }

        [Fact]
        public async Task HandleDeckPreparation_TransitionsToBetting()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.DeckPreparation);
            await Task.Delay(50);

            _mockEventManager.Verify(m => m.RaiseStateChanged(
                GameState.DeckPreparation,
                GameState.Betting), Times.Once);
        }

        #endregion

        #region State Handler Tests - Betting

        [Fact]
        public async Task HandleBetting_CallsExecuteBettingPhase()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.Betting);

            _mockRoundController.Verify(m => m.ExecuteBettingPhase(), Times.Once);
        }

        [Fact]
        public async Task HandleBetting_TransitionsToTrumpSelection()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.Betting);
            await Task.Delay(50);

            _mockEventManager.Verify(m => m.RaiseStateChanged(
                GameState.Betting,
                GameState.TrumpSelection), Times.Once);
        }

        #endregion

        #region State Handler Tests - TrumpSelection

        [Fact]
        public async Task HandleTrumpSelection_CallsSelectTrump()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.TrumpSelection);

            _mockRoundController.Verify(m => m.SelectTrump(), Times.Once);
        }

        [Fact]
        public async Task HandleTrumpSelection_ResetsTrickCounterToZero()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.TrumpSelection);
            await Task.Delay(50);

            Assert.Equal(0, _gameController.GetCurrentTrick());
        }

        [Fact]
        public async Task HandleTrumpSelection_TransitionsToPlaying()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.TrumpSelection);
            await Task.Delay(50);

            _mockEventManager.Verify(m => m.RaiseStateChanged(
                GameState.TrumpSelection,
                GameState.Playing), Times.Once);
        }

        #endregion

        #region State Handler Tests - Playing

        [Fact]
        public async Task HandlePlaying_CallsPlayAllTricks()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.Playing);

            _mockTrickController.Verify(m => m.PlayAllTricks(
                It.IsAny<int>(), 
                It.IsAny<CardSuit?>()), Times.Once);
        }

        [Fact]
        public async Task HandlePlaying_PassesCorrectStartingPlayerIndex()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);
            _mockRoundController.Setup(m => m.GetStartingPlayerIndex()).Returns(2);

            await _gameController.TransitionToState(GameState.Playing);

            _mockTrickController.Verify(m => m.PlayAllTricks(2, It.IsAny<CardSuit?>()), Times.Once);
        }

        [Fact]
        public async Task HandlePlaying_PassesCorrectTrumpSuit()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);
            _mockRoundController.Setup(m => m.TrumpSuit).Returns(CardSuit.Spades);

            await _gameController.TransitionToState(GameState.Playing);

            _mockTrickController.Verify(m => m.PlayAllTricks(It.IsAny<int>(), CardSuit.Spades), Times.Once);
        }

        [Fact]
        public async Task HandlePlaying_TransitionsToRoundEnd()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.Playing);
            await Task.Delay(50);

            _mockEventManager.Verify(m => m.RaiseStateChanged(
                GameState.Playing,
                GameState.RoundEnd), Times.Once);
        }

        #endregion

        #region State Handler Tests - RoundEnd

        [Fact]
        public async Task HandleRoundEnd_CallsFinalizeRound()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.RoundEnd);

            _mockRoundController.Verify(m => m.FinalizeRound(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task HandleRoundEnd_ChecksIfGameIsOver()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.RoundEnd);

            _mockScoringManager.Verify(m => m.IsGameOver(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task HandleRoundEnd_TransitionsToGameOver_WhenGameIsOver()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.RoundEnd);
            await Task.Delay(50);

            _mockEventManager.Verify(m => m.RaiseStateChanged(
                GameState.RoundEnd,
                GameState.GameOver), Times.Once);
        }

        [Fact]
        public async Task HandleRoundEnd_TransitionsToRoundStart_WhenGameIsNotOver()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(false);

            await _gameController.TransitionToState(GameState.RoundEnd);
            await Task.Delay(50);

            _mockEventManager.Verify(m => m.RaiseStateChanged(
                GameState.RoundEnd,
                GameState.RoundStart), Times.Once);
        }

        [Fact]
        public async Task HandleRoundEnd_IncrementsRoundNumber_WhenGameIsNotOver()
        {
            _mockScoringManager.SetupSequence(m => m.IsGameOver())
                .Returns(false)
                .Returns(true);

            await _gameController.TransitionToState(GameState.RoundEnd);
            await Task.Delay(100);

            Assert.Equal(2, _gameController.GetCurrentRound());
        }

        [Fact]
        public async Task HandleRoundEnd_DoesNotIncrementRoundNumber_WhenGameIsOver()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);

            await _gameController.TransitionToState(GameState.RoundEnd);
            await Task.Delay(100);

            Assert.Equal(0, _gameController.GetCurrentRound());
        }

        #endregion

        #region State Handler Tests - GameOver

        [Fact]
        public async Task HandleGameOver_RaisesGameOverEvent()
        {
            await _gameController.TransitionToState(GameState.GameOver);

            _mockScoringManager.Verify(m => m.RaiseGameOverEvent(), Times.Once);
        }

        #endregion

        #region Integration Flow Tests

        [Fact]
        public async Task GameFlow_ExecutesStatesInCorrectOrder()
        {
            _mockScoringManager.Setup(m => m.IsGameOver()).Returns(true);
            var stateChanges = new List<(GameState from, GameState to)>();

            _mockEventManager.Setup(m => m.RaiseStateChanged(It.IsAny<GameState>(), It.IsAny<GameState>()))
                .Callback<GameState, GameState>((from, to) => stateChanges.Add((from, to)))
                .Returns(Task.CompletedTask);

            await _gameController.TransitionToState(GameState.Initialization);
            await Task.Delay(500);

            Assert.Contains((GameState.Initialization, GameState.RoundStart), stateChanges);
            Assert.Contains((GameState.RoundStart, GameState.DeckPreparation), stateChanges);
            Assert.Contains((GameState.DeckPreparation, GameState.Betting), stateChanges);
            Assert.Contains((GameState.Betting, GameState.TrumpSelection), stateChanges);
            Assert.Contains((GameState.TrumpSelection, GameState.Playing), stateChanges);
            Assert.Contains((GameState.Playing, GameState.RoundEnd), stateChanges);
            Assert.Contains((GameState.RoundEnd, GameState.GameOver), stateChanges);
        }

        [Fact]
        public async Task GameFlow_MultipleRounds_ExecutesCorrectly()
        {
            _mockScoringManager.SetupSequence(m => m.IsGameOver())
                .Returns(false)
                .Returns(false)
                .Returns(true);

            await _gameController.TransitionToState(GameState.Initialization);
            await Task.Delay(1000);

            _mockRoundController.Verify(m => m.InitializeRound(1), Times.Once);
            _mockRoundController.Verify(m => m.InitializeRound(2), Times.Once);
            _mockRoundController.Verify(m => m.InitializeRound(3), Times.Once);
        }

        #endregion
    }
}