using DeuxCentsCardGame.Config;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces;
using DeuxCentsCardGame.Managers;
using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Controllers
{
    public class GameController : IGame
    {
        // Game state properties
        private Deck _deck;
        private bool _isGameEnded;
        private CardSuit? _trumpSuit;

        private int _currentRoundNumber = 1;

        // Managers references
        private readonly PlayerManager _playerManager;
        private readonly DealingManager _dealingManager;
        private readonly BettingManager _bettingManager;
        private readonly TrumpSelectionManager _trumpSelectionManager;
        private readonly ScoringManager _scoringManager;

        // Dealer starts at player 4 (index 3)
        public int DealerIndex = GameConfig.TeamTwoPlayer2;

        // UI reference
        private readonly IUIGameView _ui;

        // Event references
        private readonly GameEventManager _eventManager;
        private readonly GameEventHandler _eventHandler;

        public GameController(IUIGameView ui)
        {
            _ui = ui;
            _deck = new Deck();

            _eventManager = new GameEventManager();
            _eventHandler = new GameEventHandler(_eventManager, _ui);

            // Initialize managers
            _playerManager = new PlayerManager(_eventManager);
            _dealingManager = new DealingManager(_eventManager);
            _bettingManager = new BettingManager(_playerManager.Players.ToList(), DealerIndex, _eventManager);
            _trumpSelectionManager = new TrumpSelectionManager(_eventManager, _ui);
            _scoringManager = new ScoringManager(_eventManager, _playerManager.Players.ToList());

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
            _deck.ShuffleDeck();
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
            _deck = new Deck();
            _trumpSuit = null;
            _scoringManager.ResetRoundPoints();
            _bettingManager.ResetBettingRound();
            // _bettingState = new BettingManager(_players, DealerIndex, _eventManager);
        }

        private void RotateDealer()
        {
            DealerIndex = _dealingManager.RotateDealerIndex(DealerIndex, _playerManager.Players.Count);
        }

        private void DealCards()
        { 
            _dealingManager.DealCards(_deck, _playerManager.Players.ToList());
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
            string leadingSuitString = leadingSuit.HasValue ? Deck.CardSuitToString(leadingSuit.Value) : "none";
            string trumpSuitString = _trumpSuit.HasValue ? Deck.CardSuitToString(_trumpSuit.Value) : "none";

            string prompt = $"{currentPlayer.Name}, choose a card to play (enter index 0-{currentPlayer.Hand.Count - 1}" +
                (leadingSuit.HasValue ? $", leading suit is {leadingSuitString}" : "") +
                $" and trump suit is {trumpSuitString}):";

            while (true)
            {
                int cardIndex = _ui.GetIntInput(prompt, 0, currentPlayer.Hand.Count - 1);
                Card selectedCard = currentPlayer.Hand[cardIndex];

                if (selectedCard.IsPlayableCard(leadingSuit, currentPlayer.Hand))
                {
                    return selectedCard;
                }
                else
                {
                    _ui.DisplayFormattedMessage("You must play the suit of {0} since it's in your deck, try again.\n", leadingSuitString);
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
                _ui.WaitForUser("\nPress any key to start the next round...");
            }
        }
    }
}