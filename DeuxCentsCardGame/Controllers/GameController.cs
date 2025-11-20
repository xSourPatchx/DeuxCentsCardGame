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
        private readonly Dictionary<GameState, Action> _gameStateHandlers;

        // Controllers
        private readonly RoundController _roundController;
        private readonly TrickController _trickController;
        private readonly IScoringManager _scoringManager;

        // Dealer starts at player 4 (index 3)
        public int DealerIndex;

        // Event references
        private readonly IGameEventManager _eventManager;
        private readonly IGameEventHandler _eventHandler;

        public GameController(
            RoundController roundController,
            TrickController trickController,
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
            _gameStateHandlers = new Dictionary<GameState, Action>
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

        public void TransitionToState(GameState newState)
        {
            if (_gameStateData.IsPaused)
            {
                // Prevent state transitions while paused
                return;
            }

            _gameStateData.PreviousState = _gameStateData.CurrentState;
            _gameStateData.CurrentState = newState;

            // Raise event for UI/logging
            _eventManager.RaiseStateChanged(_gameStateData.PreviousState, newState);

            // Execute state handler
            if (_gameStateHandlers.TryGetValue(newState, out var handler))
            {
                handler.Invoke();
            }
        }

        // Gets the current game state (useful for Unity UI)
        public GameState GetCurrentState() => _gameStateData.CurrentState;

        // Gets the current round number
        public int GetCurrentRound() => _gameStateData.CurrentRound;

        // Gets the current trick number (within Playing state)
        public int GetCurrentTrick() => _gameStateData.CurrentTrick;

        // Pauses the game (preserves current state)
        public void PauseGame()
        {
            if (!_gameStateData.IsPaused)
            {
                _gameStateData.IsPaused = true;
                _gameStateData.StateBeforePause = _gameStateData.CurrentState;
                _eventManager.RaiseGamePaused(_gameStateData.CurrentState);
            }
        }

        // Resumes the game from pause
        public void ResumeGame()
        {
            if (_gameStateData.IsPaused)
            {
                _gameStateData.IsPaused = false;
                GameState resumeToState = _gameStateData.StateBeforePause;
                _eventManager.RaiseGameResumed(resumeToState);

                // Continue from where we left off
                TransitionToState(resumeToState);
            }
        }

        public bool IsPaused() => _gameStateData.IsPaused;

        #endregion

        #region Game Flow

        public void StartGame()
        {
            // Begin initialization state
            TransitionToState(GameState.Initialization);

            // Wait for game to end
            // In Unity, this loop would be replaced with Update() calls
            // or coroutines that check _isGameEnded
            while (!_isGameEnded)
            {
                // NewRound(); // This would be triggered by events in a real game loop
            }

            _eventHandler.UnsubscribeFromEvents();
        }

        public void NewRound()
        {
            // Start a new round
            TransitionToState(GameState.RoundStart);
        }

        #endregion

        #region State Handlers

        private void HandleInitialization()
        {
            // First-time setup
            _gameStateData.CurrentRound = 1;
            _currentRoundNumber = 1;

            // Automatically move to first round
            TransitionToState(GameState.RoundStart);
        }

        private void HandleRoundStart()
        {
            _gameStateData.CurrentRound = _currentRoundNumber;
            _roundController.InitializeRound(_currentRoundNumber);

            // Move to deck preparation
            TransitionToState(GameState.DeckPreparation);
        }

        private void HandleDeckPreparation()
        {
            _roundController.PrepareRound();

            // Move to betting phase
            TransitionToState(GameState.Betting);
        }

        private void HandleBetting()
        {
            _roundController.ExecuteBettingPhase();

            // Move to trump selection
            TransitionToState(GameState.TrumpSelection);
        }

        private void HandleTrumpSelection()
        {
            _roundController.SelectTrump();

            // Reset trick counter for new round
            _gameStateData.CurrentTrick = 0;

            // Move to playing phase
            TransitionToState(GameState.Playing);
        }

        private void HandlePlaying()
        {
            int startingPlayerIndex = _roundController.GetStartingPlayerIndex();
            _trickController.PlayAllTricks(startingPlayerIndex, _roundController.TrumpSuit);

            // Move to round end
            TransitionToState(GameState.RoundEnd);
        }

        private void HandleRoundEnd()
        {
            _roundController.FinalizeRound(_currentRoundNumber);

            // Check for game over
            if (_scoringManager.IsGameOver())
            {
                TransitionToState(GameState.GameOver);
            }
            else
            {
                _currentRoundNumber++;
                TransitionToState(GameState.RoundStart);
            }
        }
        
        private void HandleGameOver()
        {
            _scoringManager.RaiseGameOverEvent();
            _isGameEnded = true;
        }

        #endregion
    }
}