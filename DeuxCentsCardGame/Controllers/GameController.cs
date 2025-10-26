using DeuxCentsCardGame.Constants;
using DeuxCentsCardGame.Interfaces.Events;
using DeuxCentsCardGame.Interfaces.Controllers;
using DeuxCentsCardGame.Interfaces.Managers;
using DeuxCentsCardGame.Managers;
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

        // Dealer starts at player 4 (index 3)
        public int DealerIndex = GameConstants.INITIAL_DEALER_INDEX;

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
            IGameEventHandler eventHandler
        )
        {
            // Initialize managers
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _deckManager = deckManager ?? throw new ArgumentNullException(nameof(deckManager));
            _dealingManager = dealingManager ?? throw new ArgumentNullException(nameof(dealingManager));
            _bettingManager = bettingManager ?? throw new ArgumentNullException(nameof(bettingManager));
            _trumpSelectionManager = trumpSelectionManager ?? throw new ArgumentNullException(nameof(trumpSelectionManager));
            _scoringManager = scoringManager ?? throw new ArgumentNullException(nameof(scoringManager));

            // Initialize events
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));

            _trumpSuit = null;
        }

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
            ResetRound();
            _deckManager.ShuffleDeck();
            CutDeck();
            DealCards();
            ExecuteBettingRound();
            SelectTrumpSuit();
            PlayRound();
            ScoreRound();
            EndGameCheck();
            RotateDealer();
            _currentRoundNumber++;
        }

        private void ResetRound()
        {
            _deckManager.ResetDeck();
            _trumpSuit = null;
            _scoringManager.ResetRoundPoints();
            _bettingManager.ResetBettingRound();
        }

        private void RotateDealer()
        {
            DealerIndex = _dealingManager.RotateDealerIndex(DealerIndex, _playerManager.Players.Count);
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
            int currentPlayerIndex = _bettingManager.CurrentWinningBidIndex;
            PlayAllTricks(currentPlayerIndex);
        }

        private void PlayAllTricks(int startingPlayerIndex)
        {
            var startingPlayer = _playerManager.GetPlayer(startingPlayerIndex);
            int totalTricks = startingPlayer.Hand.Count;
            int currentPlayerIndex = startingPlayerIndex;

            for (int trickNumber = 0; trickNumber < totalTricks; trickNumber++)
            {
                PlaySingleTrick(currentPlayerIndex, trickNumber, out Player trickWinner, out Card trickWinningCard);

                // Set winning player as the current player for the next trick
                currentPlayerIndex = _playerManager.Players.ToList().IndexOf(trickWinner);
            }
        }

        private void PlaySingleTrick(int currentPlayerIndex, int trickNumber, out Player trickWinner, out Card trickWinningCard)
        {
            CardSuit? leadingSuit = null;
            List<(Card card, Player player)> currentTrick = [];

            PlayTrickCards(currentPlayerIndex, leadingSuit, currentTrick, trickNumber);

            (trickWinningCard, trickWinner) = DetermineTrickWinner(currentTrick, _trumpSuit);
            
            int trickPoints = CalculateTrickPoints(currentTrick);
            AwardTrickPointsAndNotify(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);
        }

        private void PlayTrickCards(int currentPlayerIndex, CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick, int trickNumber)
        {
            var players = _playerManager.Players.ToList();

            for (int trickIndex = 0; trickIndex < players.Count; trickIndex++)
            {
                int playerIndex = (currentPlayerIndex + trickIndex) % players.Count;
                Player currentPlayer = _playerManager.GetPlayer(playerIndex);

                leadingSuit = PlayPlayerTurn(currentPlayer, leadingSuit, trickNumber, currentTrick);
            }
        }

        private CardSuit? PlayPlayerTurn(Player currentPlayer, CardSuit? leadingSuit, int trickNumber, List<(Card card, Player player)> currentTrick)
        {
            _eventManager.RaisePlayerTurn(currentPlayer, leadingSuit, _trumpSuit, trickNumber);

            Card playedCard = GetValidCardFromPlayer(currentPlayer, leadingSuit);
            currentPlayer.RemoveCard(playedCard);

            // Set leading suit if this is the first card in the trick
            if (currentTrick.Count == 0)
                {
                    leadingSuit = playedCard.CardSuit;
                }

                currentTrick.Add((playedCard, currentPlayer));

                _eventManager.RaiseCardPlayed(currentPlayer, playedCard, trickNumber, leadingSuit, _trumpSuit);
                
                return leadingSuit;
        }

        private Card GetValidCardFromPlayer(Player currentPlayer, CardSuit? leadingSuit)
        {
            while (true)
            {
                int cardIndex = _eventManager.RaiseCardSelectionInput(currentPlayer, leadingSuit, _trumpSuit, currentPlayer.Hand);
                Card selectedCard = currentPlayer.Hand[cardIndex];

                if (selectedCard.IsPlayableCard(leadingSuit, currentPlayer.Hand))
                {
                    return selectedCard;
                }
                else
                {
                    string leadingSuitString = leadingSuit.HasValue ? Deck.CardSuitToString(leadingSuit.Value) : "none";
                    string message = $"You must play the suit of {leadingSuitString} since it's in your deck, try again.";

                    _eventManager.RaiseInvalidMove(currentPlayer, message, Enums.InvalidMoveType.InvalidCard);
                }
            }
        }

        private (Card winningCard, Player winningPlayer) DetermineTrickWinner(List<(Card card, Player player)> trick, CardSuit? trumpSuit)
        {
            var trickWinner = trick[0];
            CardSuit? leadingSuit = trickWinner.card.CardSuit;

            for (int i = 1; i < trick.Count; i++)
            {
                if (trick[i].card.WinsAgainst(trickWinner.card, trumpSuit, leadingSuit))
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
    }
}