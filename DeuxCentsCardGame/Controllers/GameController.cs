using DeuxCentsCardGame.Events.EventArgs;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Controllers;
using DeuxCentsCardGame.Interfaces.GameConfig;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Controllers
{
    public class GameController : IGameController
    {
        private bool _isGameEnded;
        private CardSuit? _trumpSuit;
        private int _currentRoundNumber = 1;

        // Manager dependencies injected as interfaces
        private readonly IPlayerManager _playerManager;
        private readonly IDeckManager _deckManager;
        private readonly IDealingManager _dealingManager;
        private readonly IBettingManager _bettingManager;
        private readonly ITrumpSelectionManager _trumpSelectionManager;
        private readonly IScoringManager _scoringManager;
        private readonly IGameConfig _gameConfig;

        // Dealer starts at player 4 (index 3)
        public int DealerIndex;

        // Event references
        private readonly IGameEventManager _eventManager;
        private readonly IGameEventHandler _eventHandler;

        public GameController
        (
            IPlayerManager playerManager,
            IDeckManager deckManager,
            IDealingManager dealingManager,
            IBettingManager bettingManager,
            ITrumpSelectionManager trumpSelectionManager,
            IScoringManager scoringManager,
            IGameEventManager eventManager,
            IGameEventHandler eventHandler,
            IGameConfig gameConfig
        )
        {
            // Initialize managers
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _deckManager = deckManager ?? throw new ArgumentNullException(nameof(deckManager));
            _dealingManager = dealingManager ?? throw new ArgumentNullException(nameof(dealingManager));
            _bettingManager = bettingManager ?? throw new ArgumentNullException(nameof(bettingManager));
            _trumpSelectionManager = trumpSelectionManager ?? throw new ArgumentNullException(nameof(trumpSelectionManager));
            _scoringManager = scoringManager ?? throw new ArgumentNullException(nameof(scoringManager));
            _gameConfig = gameConfig ?? throw new ArgumentNullException(nameof(gameConfig));

            // Initialize events
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));

            _trumpSuit = null;
            DealerIndex = _gameConfig.InitialDealerIndex;
        }

        #region Game Flow

        public void StartGame()
        {
            while (!_isGameEnded)
            {
                NewRound();
            }

            _eventHandler.UnsubscribeFromEvents();
        }

        public void NewRound()
        {
            _eventManager.RaiseRoundStarted(_currentRoundNumber, _playerManager.GetPlayer(DealerIndex));
            InitializeRound();
            PrepareGameState();
            ExecuteRoundPhases();
            FinalizeRound();
            _currentRoundNumber++;
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
            // Player to the right of dealer cuts (dealer - 1, wrapping around)
            int cuttingPlayerIndex = (DealerIndex - 1 + _playerManager.Players.Count) % _playerManager.Players.Count;
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

        private void ExecuteRoundPhases()
        {
            ExecuteBettingRound();
            SelectTrumpSuit();
            PlayRound();
        }

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
            int currentPlayerIndex = startingPlayerIndex;

            for (int trickNumber = 0; trickNumber < totalTricks; trickNumber++)
            {
                currentPlayerIndex = ExecuteSingleTrick(currentPlayerIndex, trickNumber);
            }
        }

        private int ExecuteSingleTrick(int currentPlayerIndex, int trickNumber)
        {
            CardSuit? leadingSuit = null;
            List<(Card card, Player player)> currentTrick = [];

            PlayTrickCards(currentPlayerIndex, ref leadingSuit, currentTrick, trickNumber);

            var (trickWinningCard, trickWinner) = DetermineTrickWinner(currentTrick);
            int trickPoints = CalculateTrickPoints(currentTrick);
            AwardTrickPointsAndNotify(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);

            // Return winning player index for next trick
            return _playerManager.Players.ToList().IndexOf(trickWinner);
        }

        #endregion

        #region Card Play Logic

        private void PlayTrickCards(int currentPlayerIndex, ref CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick, int trickNumber)
        {
            var players = _playerManager.Players.ToList();

            for (int trickIndex = 0; trickIndex < players.Count; trickIndex++)
            {
                int playerIndex = (currentPlayerIndex + trickIndex) % players.Count;
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
            string leadingSuitString = leadingSuit.HasValue ? Deck.CardSuitToString(leadingSuit.Value) : "none";
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

        private void AwardTrickPointsAndNotify(int trickNumber, Player trickWinner, Card trickWinningCard, List<(Card card, Player player)> currentTrick, int trickPoints)
        {
            _scoringManager.AwardTrickPoints(_playerManager.Players.ToList().IndexOf(trickWinner), trickPoints);
            _eventManager.RaiseTrickCompleted(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);
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

            if (_scoringManager.IsGameOver())
            {
                _scoringManager.RaiseGameOverEvent();
                _isGameEnded = true;
            }
            else
            {
                _eventManager.RaiseNextRoundPrompt();
            }
        }

        private void RotateDealer()
        {
            DealerIndex = _dealingManager.RotateDealerIndex(DealerIndex, _playerManager.Players.Count);
        }

        #endregion
    }
}