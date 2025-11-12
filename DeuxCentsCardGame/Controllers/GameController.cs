using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.GameStates;
using DeuxCentsCardGame.Interfaces.Controllers;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Interfaces.Services;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Controllers
{
    public class GameController : IGameController
    {
        private bool _isGameEnded;
        private CardSuit? _trumpSuit;
        private int _currentRoundNumber = 1;

        // State management
        private readonly GameStateData _gameStateData;
        private readonly Dictionary<GameState, Action> _gameStateHandlers;

        // Manager dependencies injected as interfaces
        private readonly IGameConfig _gameConfig;
        private readonly ICardUtility _cardUtility;
        private readonly IPlayerManager _playerManager;
        private readonly IPlayerTurnManager _playerTurnManager;
        private readonly IDeckManager _deckManager;
        private readonly IDealingManager _dealingManager;
        private readonly IBettingManager _bettingManager;
        private readonly ITrumpSelectionManager _trumpSelectionManager;
        private readonly IScoringManager _scoringManager;

        // Dealer starts at player 4 (index 3)
        public int DealerIndex;

        // Event references
        private readonly IGameEventManager _eventManager;
        private readonly IGameEventHandler _eventHandler;

        public GameController(
            IGameConfig gameConfig,
            ICardUtility cardUtility,
            IPlayerManager playerManager,
            IPlayerTurnManager playerTurnManager,
            IDeckManager deckManager,
            IDealingManager dealingManager,
            IBettingManager bettingManager,
            ITrumpSelectionManager trumpSelectionManager,
            IScoringManager scoringManager,
            IGameEventManager eventManager,
            IGameEventHandler eventHandler)
        {
            // Initialize configs and utilities
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));
            _cardUtility = cardUtility ?? throw new ArgumentNullException(nameof(cardUtility));

            // Initialize managers
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _playerTurnManager = playerTurnManager as IPlayerTurnManager ?? throw new ArgumentNullException(nameof(playerTurnManager));
            _deckManager = deckManager ?? throw new ArgumentNullException(nameof(deckManager));
            _dealingManager = dealingManager ?? throw new ArgumentNullException(nameof(dealingManager));
            _bettingManager = bettingManager ?? throw new ArgumentNullException(nameof(bettingManager));
            _trumpSelectionManager = trumpSelectionManager ?? throw new ArgumentNullException(nameof(trumpSelectionManager));
            _scoringManager = scoringManager ?? throw new ArgumentNullException(nameof(scoringManager));

            // Initialize events
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));

            _trumpSuit = null;
            DealerIndex = _gameConfig.InitialDealerIndex;

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
            _eventManager.RaiseRoundStarted(_currentRoundNumber, _playerManager.GetPlayer(DealerIndex));

            InitializeRound();

            // Move to deck preparation
            TransitionToState(GameState.DeckPreparation);
        }

        private void HandleDeckPreparation()
        {
            PrepareGameState();

            // Move to betting phase
            TransitionToState(GameState.Betting);
        }

        private void HandleBetting()
        {
            ExecuteBettingRound();

            // Move to trump selection
            TransitionToState(GameState.TrumpSelection);
        }

        private void HandleTrumpSelection()
        {
            SelectTrumpSuit();

            // Reset trick counter for new round
            _gameStateData.CurrentTrick = 0;

            // Move to playing phase
            TransitionToState(GameState.Playing);
        }

        private void HandlePlaying()
        {
            PlayRound();

            // Move to round end
            TransitionToState(GameState.RoundEnd);
        }

        private void HandleRoundEnd()
        {
            FinalizeRound();

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

        #region Round Initialization

        private void InitializeRound()
        {
            ResetRound();
        }

        private void ResetRound()
        {
            _deckManager.ResetDeck();
            _trumpSuit = null;
            _scoringManager.ResetRoundPoints();
            _bettingManager.ResetBettingRound();
            _playerTurnManager.ResetTurnSequence();
        }

        #endregion

        #region Game State Preparation

        private void PrepareGameState()
        {
            ShuffleDeck();
            CutDeck();
            DealCards();
        }

        private void ShuffleDeck()
        {
            _deckManager.ShuffleDeck();
        }

        private void CutDeck()
        {
            // Player to the right of dealer cuts
            int cuttingPlayerIndex = _playerTurnManager.GetPlayerRightOfDealer(DealerIndex);
            Player cuttingPlayer = _playerManager.GetPlayer(cuttingPlayerIndex);
            
            int deckSize = _deckManager.CurrentDeck.Cards.Count;
            int cutPosition = _eventManager.RaiseDeckCutInput(cuttingPlayer, deckSize);
            
            _deckManager.CutDeck(cutPosition);
            _eventManager.RaiseDeckCut(cuttingPlayer, cutPosition);
        }

        private void DealCards()
        { 
            _dealingManager.DealCards(_deckManager.CurrentDeck, _playerManager.Players.ToList());
            _dealingManager.RaiseCardsDealtEvent(_playerManager.Players.ToList(), DealerIndex);
        }

        #endregion

        #region Round Execution

        public void ExecuteBettingRound()
        {
            _bettingManager.UpdateDealerIndex(DealerIndex);
            _bettingManager.ExecuteBettingRound();
        }

        private void SelectTrumpSuit()
        {
            _trumpSuit = _trumpSelectionManager.SelectTrumpSuit(_playerManager.GetPlayer(_bettingManager.CurrentWinningBidIndex));
        }

        private void PlayRound()
        {
            int startingPlayerIndex = _bettingManager.CurrentWinningBidIndex;
            PlayAllTricks(startingPlayerIndex);
        }

        #endregion

        #region Trick Management

        private void PlayAllTricks(int startingPlayerIndex)
        {
            var startingPlayer = _playerManager.GetPlayer(startingPlayerIndex);
            int totalTricks = startingPlayer.Hand.Count;

            // Initialize turn manager for the round
            _playerTurnManager.InitializeTurnSequence(startingPlayerIndex);

            for (int trickNumber = 0; trickNumber < totalTricks; trickNumber++)
            {
                _gameStateData.CurrentTrick = trickNumber;
                ExecuteSingleTrick(trickNumber);
            }
        }

        private void ExecuteSingleTrick(int trickNumber)
        {
            CardSuit? leadingSuit = null;
            List<(Card card, Player player)> currentTrick = [];

            PlayTrickCards(ref leadingSuit, currentTrick, trickNumber);

            var (trickWinningCard, trickWinner) = DetermineTrickWinner(currentTrick);
            int trickPoints = CalculateTrickPoints(currentTrick);
            int trickWinnerIndex = AwardTrickPointsAndNotify(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);

            // Update turn manager for next trick
            _playerTurnManager.SetCurrentPlayer(trickWinnerIndex);
        }

        #endregion

        #region Card Play Logic

        private void PlayTrickCards(ref CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick, int trickNumber)
        {
            var players = _playerManager.Players.ToList();
            var turnOrder = _playerTurnManager.GetTurnOrder();

            foreach (int playerIndex in turnOrder)
            {
                Player currentPlayer = _playerManager.GetPlayer(playerIndex);
                ExecutePlayerTurn(currentPlayer, ref leadingSuit, trickNumber, currentTrick);
            }
        }

        private void ExecutePlayerTurn(Player currentPlayer, ref CardSuit? leadingSuit, int trickNumber, List<(Card card, Player player)> currentTrick)
        {
            RaisePlayerTurnEvent(currentPlayer, leadingSuit, trickNumber);
            Card playedCard = GetValidCardFromPlayer(currentPlayer, leadingSuit);
            currentPlayer.RemoveCard(playedCard);
            UpdateLeadingSuit(playedCard, ref leadingSuit, currentTrick);
            currentTrick.Add((playedCard, currentPlayer));
            RaiseCardPlayedEvent(currentPlayer, playedCard, trickNumber, leadingSuit);
        }

        private void RaisePlayerTurnEvent(Player currentPlayer, CardSuit? leadingSuit, int trickNumber)
        {
            _eventManager.RaisePlayerTurn(currentPlayer, leadingSuit, _trumpSuit, trickNumber);
        }

        private void UpdateLeadingSuit(Card playedCard, ref CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick)
        {
            // Set leading suit if this is the first card in the trick
            if (currentTrick.Count == 0)
            {
                leadingSuit = playedCard.CardSuit;
            }
        }

        private void RaiseCardPlayedEvent(Player currentPlayer, Card playedCard, int trickNumber, CardSuit? leadingSuit)
        {
            _eventManager.RaiseCardPlayed(currentPlayer, playedCard, trickNumber, leadingSuit, _trumpSuit);
        }

        #endregion

        #region Card Selection and Validation

        private Card GetValidCardFromPlayer(Player currentPlayer, CardSuit? leadingSuit)
        {
            while (true)
            {
                int cardIndex = RequestCardSelection(currentPlayer, leadingSuit);
                Card selectedCard = currentPlayer.Hand[cardIndex];

                if (IsCardValid(selectedCard, leadingSuit, currentPlayer.Hand))
                {
                    return selectedCard;
                }

                DisplayInvalidCardMessage(currentPlayer, leadingSuit);
            }
        }

        private int RequestCardSelection(Player currentPlayer, CardSuit? leadingSuit)
        {
            return _eventManager.RaiseCardSelectionInput(currentPlayer, leadingSuit, _trumpSuit, currentPlayer.Hand);
        }

        private bool IsCardValid(Card selectedCard, CardSuit? leadingSuit, List<Card> hand)
        {
            return selectedCard.IsPlayableCard(leadingSuit, hand);
        }

        private void DisplayInvalidCardMessage(Player currentPlayer, CardSuit? leadingSuit)
        {
            string leadingSuitString = leadingSuit.HasValue ? _cardUtility.CardSuitToString(leadingSuit.Value) : "none";
            string message = $"You must play the suit of {leadingSuitString} since it's in your deck, try again.";

            _eventManager.RaiseInvalidMove(currentPlayer, message, InvalidMoveType.InvalidCard);
        }

        #endregion

        #region Trick Resolution

        private (Card winningCard, Player winningPlayer) DetermineTrickWinner(List<(Card card, Player player)> trick)
        {
            var trickWinner = trick[0];
            CardSuit? leadingSuit = trickWinner.card.CardSuit;

            for (int i = 1; i < trick.Count; i++)
            {
                if (trick[i].card.WinsAgainst(trickWinner.card, _trumpSuit, leadingSuit))
                {
                    trickWinner = trick[i];
                }
            }

            return (trickWinner.card, trickWinner.player);
        }

        private int CalculateTrickPoints(List<(Card card, Player player)> trick)
        {
            return trick.Sum(trick => trick.card.CardPointValue);
        }

        private int AwardTrickPointsAndNotify(int trickNumber, Player trickWinner, Card trickWinningCard, List<(Card card, Player player)> currentTrick, int trickPoints)
        {
            int winnerIndex = _playerManager.Players.ToList().IndexOf(trickWinner);
            _scoringManager.AwardTrickPoints(winnerIndex, trickPoints);
            _eventManager.RaiseTrickCompleted(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);
            return winnerIndex;
        }

        #endregion

        #region Round Finalization

        private void FinalizeRound()
        {
            ScoreRound();
            EndGameCheck();
            RotateDealer();
        }

        private void ScoreRound()
        {
            _scoringManager.ScoreRound(_bettingManager.CurrentWinningBidIndex, _bettingManager.CurrentWinningBid);
        }

        private void EndGameCheck()
        {
            _eventManager.RaiseRoundEnded(
                _currentRoundNumber,
                _scoringManager.TeamOneRoundPoints,
                _scoringManager.TeamTwoRoundPoints,
                _playerManager.GetPlayer(_bettingManager.CurrentWinningBidIndex),
                _bettingManager.CurrentWinningBid
            );

            if (!_scoringManager.IsGameOver())
            {
                _eventManager.RaiseNextRoundPrompt();
            }
        }

        private void RotateDealer()
        {
            DealerIndex = _playerTurnManager.RotateDealer(DealerIndex);
        }

        #endregion
    }
}