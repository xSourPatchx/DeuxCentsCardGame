using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces.Controllers;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Controllers
{
    public class GameController : IGameController
    {
        private bool _isGameEnded;
        private CardSuit? _trumpSuit;
        private int _currentRoundNumber = 1;

        // Managers dependencies injected
        private readonly PlayerManager _playerManager;
        private readonly DeckManager _deckManager;
        private readonly DealingManager _dealingManager;
        private readonly BettingManager _bettingManager;
        private readonly TrumpSelectionManager _trumpSelectionManager;
        private readonly ScoringManager _scoringManager;

        // Dealer starts at player 4 (index 3)
        public int DealerIndex = 3;


        // Event references
        private readonly GameEventManager _eventManager;
        private readonly GameEventHandler _eventHandler;

        public GameController
        (
            PlayerManager playerManager,
            DeckManager deckManager,
            DealingManager dealingManager,
            BettingManager bettingManager,
            TrumpSelectionManager trumpSelectionManager,
            ScoringManager scoringManager,
            GameEventManager eventManager,
            GameEventHandler eventHandler
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

        private void DealCards()
        { 
            _dealingManager.DealCards(_deckManager.CurrentDeck, _playerManager.Players.ToList());
            _dealingManager.RaiseCardsDealtEvent(_playerManager.Players.ToList(), DealerIndex);
        }

        public void ExecuteBettingRound()
        {
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
            Card trickWinningCard;
            Player trickWinner;

            for (int trickNumber = 0; trickNumber < totalTricks; trickNumber++)
            {
                CardSuit? leadingSuit = null;
                List<(Card card, Player player)> currentTrick = [];

                PlayTrick(currentPlayerIndex, leadingSuit, currentTrick, trickNumber);

                (trickWinningCard, trickWinner) = DetermineTrickWinner(currentTrick, _trumpSuit);

                // Set winning player as the current player for the next trick
                currentPlayerIndex = _playerManager.Players.ToList().IndexOf(trickWinner);

                int trickPoints = currentTrick.Sum(trick => trick.card.CardPointValue);

                _eventManager.RaiseTrickCompleted(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);

                _scoringManager.AwardTrickPoints(currentPlayerIndex, trickPoints);
            }
        }

        private void PlayTrick(int currentPlayerIndex, CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick, int trickNumber)
        {
            var players = _playerManager.Players.ToList();

            for (int trickIndex = 0; trickIndex < players.Count; trickIndex++)
            {
                // Ensuring player who won the bet goes first
                int playerIndex = (currentPlayerIndex + trickIndex) % players.Count;
                Player currentPlayer = _playerManager.GetPlayer(playerIndex);

                // Raise event for player's turn and display their hand
                _eventManager.RaisePlayerTurn(currentPlayer, leadingSuit, _trumpSuit, trickNumber);

                Card playedCard = GetValidCardFromPlayer(currentPlayer, leadingSuit);
                currentPlayer.RemoveCard(playedCard);

                if (trickIndex == 0)
                {
                    leadingSuit = playedCard.CardSuit;
                }

                currentTrick.Add((playedCard, currentPlayer));

                _eventManager.RaiseCardPlayed(currentPlayer, playedCard, trickNumber, leadingSuit, _trumpSuit);
            }
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
                    _eventManager.RaiseInvalidCard($"You must play the suit of {leadingSuitString} since it's in your deck, try again.");
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

        private void ScoreRound()
        {
            _scoringManager.ScoreRound(_bettingManager.CurrentWinningBidIndex, _bettingManager.CurrentWinningBid);
        }

        private void EndGameCheck()
        {
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