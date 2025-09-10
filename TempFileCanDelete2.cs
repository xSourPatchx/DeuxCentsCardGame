using DeuxCentsCardGame.Config;
using DeuxCentsCardGame.Events;
using DeuxCentsCardGame.Interfaces;
using DeuxCentsCardGame.Models;
using DeuxCentsCardGame.Services;

namespace DeuxCentsCardGame.Controllers
{
    public class GameController : IGame
    {
        // Game state properties
        private Deck _deck;
        private readonly List<Player> _players;
        private bool _isGameEnded;
        private CardSuit? _trumpSuit;
        private int _currentRoundNumber = 1;

        // Services
        private readonly DealingService _dealingService;
        private readonly TrumpSelectionService _trumpSelectionService;
        private readonly ScoringService _scoringService;
        private BettingService _bettingService;

        // dealer starts at player 4 (index 3)
        public int DealerIndex = GameConfig.TeamTwoPlayer2;

        // UI and Event references
        private readonly IUIGameView _ui;
        private readonly GameEventManager _eventManager;
        private readonly GameEventHandler _eventHandler;

        public GameController(IUIGameView ui)
        {
            _ui = ui;
            _deck = new Deck();
            _players =
            [
                new("Player 1"),
                new("Player 2"),
                new("Player 3"),
                new("Player 4"),
            ];

            _trumpSuit = null;
            _eventManager = new GameEventManager();
            _eventHandler = new GameEventHandler(_eventManager, _ui);

            // Initialize services
            _dealingService = new DealingService(_eventManager);
            _trumpSelectionService = new TrumpSelectionService(_eventManager, _ui);
            _scoringService = new ScoringService(_eventManager, _players);
            _bettingService = new BettingService(_players, DealerIndex, _eventManager);
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
            _eventManager.RaiseRoundStarted(_currentRoundNumber, _players[DealerIndex]);
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
            _scoringService.ResetRoundPoints();
            _bettingService = new BettingService(_players, DealerIndex, _eventManager);
        }

        private void RotateDealer()
        {
            DealerIndex = _dealingService.GetNextDealerIndex(DealerIndex, _players.Count);
        }

        private void DealCards()
        {
            _dealingService.DealCards(_deck, _players);
            _dealingService.RaiseCardsDealtEvent(_players, DealerIndex);
        }

        public void ExecuteBettingRound()
        {
            _bettingService.ResetBettingRound();
            _bettingService.ExecuteBettingRound();
        }

        private void SelectTrumpSuit()
        {
            Player winningBidder = _players[_bettingService.CurrentWinningBidIndex];
            _trumpSuit = _trumpSelectionService.SelectTrumpSuit(winningBidder);
        }

        private void PlayRound()
        {
            int currentPlayerIndex = _bettingService.CurrentWinningBidIndex;
            PlayAllTricks(currentPlayerIndex);
        }

        private void PlayAllTricks(int startingPlayerIndex)
        {
            int totalTricks = _players[startingPlayerIndex].Hand.Count;
            int currentPlayerIndex = startingPlayerIndex;
            Card trickWinningCard;
            Player trickWinner;

            for (int trickNumber = 0; trickNumber < totalTricks; trickNumber++)
            {
                CardSuit? leadingSuit = null;
                List<(Card card, Player player)> currentTrick = [];

                PlayTrick(currentPlayerIndex, leadingSuit, currentTrick, trickNumber);

                (trickWinningCard, trickWinner) = DetermineTrickWinner(currentTrick, _trumpSuit);
                currentPlayerIndex = _players.IndexOf(trickWinner);

                int trickPoints = currentTrick.Sum(trick => trick.card.CardPointValue);

                _eventManager.RaiseTrickCompleted(trickNumber, trickWinner, trickWinningCard, currentTrick, trickPoints);
                _scoringService.AwardTrickPoints(currentPlayerIndex, trickPoints);
            }
        }

        private void PlayTrick(int currentPlayerIndex, CardSuit? leadingSuit, List<(Card card, Player player)> currentTrick, int trickNumber)
        {
            for (int trickIndex = 0; trickIndex < _players.Count; trickIndex++)
            {
                int playerIndex = (currentPlayerIndex + trickIndex) % _players.Count;
                Player currentPlayer = _players[playerIndex];

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

        private static (Card winningCard, Player winningPlayer) DetermineTrickWinner(List<(Card card, Player player)> trick, CardSuit? trumpSuit)
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
            _scoringService.ScoreRound(_bettingService.CurrentWinningBidIndex, _bettingService.CurrentWinningBid);
        }

        private void EndGameCheck()
        {
            if (_scoringService.IsGameOver())
            {
                _scoringService.RaiseGameOverEvent();
                _isGameEnded = true;
            }
            else
            {
                _ui.WaitForUser("\nPress any key to start the next round...");
            }
        }
    }
}