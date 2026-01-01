using DeuxCentsCardGame.GameStates;
using DeuxCentsCardGame.Interfaces.Controllers;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Managers;

namespace DeuxCentsCardGame.Controllers
{
    public class GameController : IGameController
    {
        private bool _isGameEnded;
        private int _currentRoundNumber = 1;

        // State management
        private readonly GameStateData _gameStateData;
        private readonly Dictionary<GameState, Func<Task>> _gameStateHandlers;

        // Controllers
        private readonly IRoundController _roundController;
        private readonly ITrickController _trickController;
        private readonly IScoringManager _scoringManager;

        // Dealer starts at player 4 (index 3)
        public int DealerIndex;

        // Event references
        private readonly IGameEventManager _eventManager;
        private readonly IGameEventHandler _eventHandler;

        public GameController(
            IRoundController roundController,
            ITrickController trickController,
            IScoringManager scoringManager,
            IGameEventManager eventManager,
            IGameEventHandler eventHandler)
        {
            // Initialize Controllers
            _roundController = roundController ?? throw new ArgumentNullException(nameof(roundController));
            _trickController = trickController ?? throw new ArgumentNullException(nameof(trickController));
            _scoringManager = scoringManager ?? throw new ArgumentNullException(nameof(scoringManager));

            // Initialize events
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));

            // Initialize state system
            _gameStateData = new GameStateData();

            // Map states to handlers
            _gameStateHandlers = new Dictionary<GameState, Func<Task>>
            {
                { GameState.Initialization, HandleInitialization },
                { GameState.RoundStart, HandleRoundStart },
                { GameState.DeckPreparation, HandleDeckPreparation },
                { GameState.Betting, HandleBetting },
                { GameState.TrumpSelection, HandleTrumpSelection },
                { GameState.Playing, HandlePlaying },
                { GameState.RoundEnd, HandleRoundEnd },
                { GameState.GameOver, HandleGameOver }
            };
        }
  
        #region State Management

        public async Task TransitionToState(GameState newState)
        {
            if (_gameStateData.IsPaused)
            {
                // Prevent state transitions while paused
                return;
            }

            _gameStateData.PreviousState = _gameStateData.CurrentState;
            _gameStateData.CurrentState = newState;

            // Raise event for UI/logging
            await _eventManager.RaiseStateChanged(_gameStateData.PreviousState, newState);

            // Execute state handler
            if (_gameStateHandlers.TryGetValue(newState, out var handler))
            {
                await handler.Invoke();
            }
        }

        // Gets the current game state (useful for Unity UI)
        public GameState GetCurrentState() => _gameStateData.CurrentState;

        // Gets the current round number
        public int GetCurrentRound() => _gameStateData.CurrentRound;

        // Gets the current trick number (within Playing state)
        public int GetCurrentTrick() => _gameStateData.CurrentTrick;

        // Pauses the game (preserves current state)
        public async Task PauseGame()
        {
            if (!_gameStateData.IsPaused)
            {
                _gameStateData.IsPaused = true;
                _gameStateData.StateBeforePause = _gameStateData.CurrentState;
                await _eventManager.RaiseGamePaused(_gameStateData.CurrentState);
            }
        }

        // Resumes the game from pause
        public async Task ResumeGame()
        {
            if (_gameStateData.IsPaused)
            {
                _gameStateData.IsPaused = false;
                GameState resumeToState = _gameStateData.StateBeforePause;
                await _eventManager.RaiseGameResumed(resumeToState);

                // Continue from where we left off
                await TransitionToState(resumeToState);
            }
        }

        public bool IsPaused() => _gameStateData.IsPaused;

        #endregion

        #region Game Flow

        public async Task StartGame()
        {
            // Begin initialization state
            await TransitionToState(GameState.Initialization);

            // Wait for game to end
            // In Unity, this loop would be replaced with Update() calls
            // or coroutines that check _isGameEnded
            while (!_isGameEnded)
            {
                // NewRound(); // This would be triggered by events in a real game loop
                await Task.Delay(100); // Prevent tight loop in  context
            }

            _eventHandler.UnsubscribeFromEvents();
        }

        public async Task NewRound()
        {
            // Start a new round
            await TransitionToState(GameState.RoundStart);
        }

        #endregion

        #region State Handlers

        private async Task HandleInitialization()
        {
            // First-time setup
            _gameStateData.CurrentRound = 1;
            _currentRoundNumber = 1;

            // Automatically move to first round
            await TransitionToState(GameState.RoundStart);
        }

        private async Task HandleRoundStart()
        {
            _gameStateData.CurrentRound = _currentRoundNumber;
            await _roundController.InitializeRound(_currentRoundNumber);

            // Move to deck preparation
            await TransitionToState(GameState.DeckPreparation);
        }

        private async Task HandleDeckPreparation()
        {
            await _roundController.PrepareRound();

            // Move to betting phase
            await TransitionToState(GameState.Betting);
        }

        private async Task HandleBetting()
        {
            await _roundController.ExecuteBettingPhase();

            // Move to trump selection
            await TransitionToState(GameState.TrumpSelection);
        }

        private async Task HandleTrumpSelection()
        {
            await _roundController.SelectTrump();

            // Reset trick counter for new round
            _gameStateData.CurrentTrick = 0;

            // Move to playing phase
            await TransitionToState(GameState.Playing);
        }

        private async Task HandlePlaying()
        {
            int startingPlayerIndex = _roundController.GetStartingPlayerIndex();
            await _trickController.PlayAllTricks(startingPlayerIndex, _roundController.TrumpSuit);

            // Move to round end
            await TransitionToState(GameState.RoundEnd);
        }

        private async Task HandleRoundEnd()
        {
            await _roundController.FinalizeRound(_currentRoundNumber);

            // Check for game over
            if (_scoringManager.IsGameOver())
            {
                await TransitionToState(GameState.GameOver);
            }
            else
            {
                _currentRoundNumber++;
                await TransitionToState(GameState.RoundStart);
            }
        }
        
        private async Task HandleGameOver()
        {
            _scoringManager.RaiseGameOverEvent();
            _isGameEnded = true;
            await Task.CompletedTask;
        }

        #endregion
    }
}